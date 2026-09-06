using IglesiaBackend.Data;
using IglesiaBackend.Features.RegistryEvents;
using IglesiaBackend.Features.Reports.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace IglesiaBackend.Features.Reports;

public class ReportRepository
{
    private readonly AppDbContext _context;

    public ReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Report>> GetAllAsync()
    {
        return await _context.Reports
            .AsNoTracking()
            .IgnoreQueryFilters()
            .OrderByDescending(r => r.Id)
            .ToListAsync();
    }

    public async Task<Report?> GetByIdAsync(int id)
    {
        return await _context.Reports
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddAsync(Report entity)
    {
        _context.Reports.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Report entity)
    {
        _context.Reports.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Report entity)
    {
        _context.Reports.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSimpleAsync(Report entity)
    {
        _context.Reports.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ReportColumnDto>> GetAvailableColumnsAsync(int recordTypeId)
    {
        var columns = new List<ReportColumnDto>
        {
            new ReportColumnDto { Key = "registryDate", Label = "Fecha de Registro", IsDynamic = false, IsRequired = true },
            new ReportColumnDto { Key = "eventName", Label = "Evento", IsDynamic = false, IsRequired = true },
            new ReportColumnDto { Key = "structureName", Label = "Red / Estructura", IsDynamic = false, IsRequired = true },
            new ReportColumnDto { Key = "leaderName", Label = "Líder Responsable", IsDynamic = false, IsRequired = true }
        };

        var dynamicFields = await _context.RecordTypeFields
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(f => f.RecordTypeId == recordTypeId && !f.IsDeleted)
            .OrderBy(f => f.FieldOrder)
            .Select(f => new ReportColumnDto 
            { 
                Key = f.Name, 
                Label = f.Label, 
                IsDynamic = true, 
                IsRequired = false,
                DataType = f.DataType.ToUpper()
            })
            .ToListAsync();

        var uniqueDynamicFields = dynamicFields.GroupBy(f => f.Key).Select(g => g.First()).ToList();
        columns.AddRange(uniqueDynamicFields);

        return columns;
    }

    // =========================================================
    // GENERACIÓN PLANO Y MATRICIAL POR SEMANAS CON TOTALIZADOR
    // =========================================================
    public async Task<List<Dictionary<string, object>>> GenerateFlatReportAsync(DynamicReportRequestDto request)
    {
        var resultData = new List<Dictionary<string, object>>();

        var columnLabels = await GetAvailableColumnsAsync(request.RecordTypeId);
        var labelMap = columnLabels.ToDictionary(c => c.Key, c => c.Label);

        var query = _context.RegistryEvents
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.Event).ThenInclude(e => e.OrganizationStructure)
            .Include(x => x.Leader)
            .Where(x => x.RecordTypeId == request.RecordTypeId && !x.IsDeleted);

        // Filtro por Rango de Fechas
        if (request.StartDate.HasValue)
            query = query.Where(x => x.RegistryDate >= request.StartDate.Value);

        if (request.EndDate.HasValue)
        {
            var endOfDay = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(x => x.RegistryDate <= endOfDay);
        }

        // 🔥 FILTRO POR ESTRUCTURA ORGANIZACIONAL (RED/MINISTERIO)
        if (request.StructureId.HasValue && request.StructureId.Value > 0)
        {
            query = query.Where(x => x.Event != null && x.Event.OrganizationStructureId == request.StructureId.Value);
        }

        var rawData = await query.OrderBy(x => x.RegistryDate).ToListAsync();

        // 1. Detección de semanas y asignación de etiquetas "Semana N (DD/MM - DD/MM)"
        foreach (var row in rawData)
        {
            var flatRow = new Dictionary<string, object>();
            var jsonElements = row.DataJson.RootElement;
            var regDate = row.RegistryDate.ToLocalTime();

            // Cálculo de rango de la semana
            var startOfWeek = regDate.Date.AddDays(-(int)regDate.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(6);
            string weekTag = $"Semana ({startOfWeek:dd/MM} - {endOfWeek:dd/MM})";

            flatRow["Semana"] = weekTag;

            foreach (var colKey in request.SelectedColumns)
            {
                string headerLabel = labelMap.TryGetValue(colKey, out var lbl) ? lbl : colKey;

                if (colKey == "registryDate") flatRow[headerLabel] = regDate.ToString("yyyy-MM-dd HH:mm");
                else if (colKey == "eventName") flatRow[headerLabel] = row.Event?.Name ?? "N/A";
                else if (colKey == "structureName") flatRow[headerLabel] = row.Event?.OrganizationStructure?.Name ?? "General";
                else if (colKey == "leaderName") flatRow[headerLabel] = row.Leader != null ? $"{row.Leader.FirstName} {row.Leader.LastName}" : "N/A";
                else
                {
                    if (jsonElements.TryGetProperty(colKey, out var element))
                    {
                        flatRow[headerLabel] = element.ValueKind switch
                        {
                            JsonValueKind.String => element.GetString() ?? "",
                            JsonValueKind.Number => element.GetDecimal(),
                            JsonValueKind.True => "Sí",
                            JsonValueKind.False => "No",
                            _ => element.ToString()
                        };
                    }
                    else
                    {
                        flatRow[headerLabel] = "";
                    }
                }
            }
            resultData.Add(flatRow);
        }

        // 🔥 FILA DE TOTALIZADOR GENERAL
        if (resultData.Count > 0)
        {
            var totalRow = new Dictionary<string, object>();
            totalRow["Semana"] = "TOTALES ACUMULADOS";

            var numericColumns = resultData.First().Keys.Where(k => k != "Semana").ToList();

            foreach (var key in numericColumns)
            {
                decimal sum = 0;
                bool isNumeric = false;

                foreach (var row in resultData)
                {
                    if (row.TryGetValue(key, out var val) && val is decimal numVal)
                    {
                        sum += numVal;
                        isNumeric = true;
                    }
                }

                totalRow[key] = isNumeric ? sum : "---";
            }

            resultData.Add(totalRow);
        }

        return resultData;
    }
}