using IglesiaBackend.Data;
using IglesiaBackend.Features.OrganizationMembers; // Para acceder a los roles del líder
using IglesiaBackend.Features.RegistryEvents;
using IglesiaBackend.Features.Reports.Dtos;
using Microsoft.EntityFrameworkCore;
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
    public async Task<List<Dictionary<string, object>>> GenerateFlatReportAsync(DynamicReportRequestDto request, string userRole, int? currentMemberId)
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

        // 1. FILTRO DE FECHAS
        if (request.StartDate.HasValue)
            query = query.Where(x => x.RegistryDate >= request.StartDate.Value);

        if (request.EndDate.HasValue)
        {
            var endOfDay = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(x => x.RegistryDate <= endOfDay);
        }

        // 🔥 2. SEGURIDAD A NIVEL DE FILA Y FILTRO JERÁRQUICO
        var normalizedRole = userRole.ToUpper();
        bool isSuperAdmin = normalizedRole == "SUPERADMIN" || normalizedRole == "ADMIN";
        
        List<int> allowedStructureIds = new List<int>();
        var allStructs = await _context.OrganizationStructures.AsNoTracking().ToListAsync();

        // Función recursiva para obtener células hijas
        void GetChildrenStructures(int parentId, List<int> targetList)
        {
            var children = allStructs.Where(s => s.ParentId == parentId).Select(s => s.Id).ToList();
            foreach (var child in children)
            {
                if (!targetList.Contains(child))
                {
                    targetList.Add(child);
                    GetChildrenStructures(child, targetList);
                }
            }
        }

        // Si es Líder/Facilitador, descubrimos su árbol permitido
        if (!isSuperAdmin && currentMemberId.HasValue)
        {
            var userStructures = await _context.OrganizationMembers
                .Where(om => om.MemberId == currentMemberId.Value)
                .Select(om => om.OrganizationStructureId)
                .ToListAsync();

            foreach (var us in userStructures)
            {
                if (!allowedStructureIds.Contains(us)) allowedStructureIds.Add(us);
                GetChildrenStructures(us, allowedStructureIds);
            }

            if (!allowedStructureIds.Any()) return new List<Dictionary<string, object>>(); // No tiene red, no ve reportes
        }

        // Resolvemos el filtro del Dropdown del usuario
        List<int> filterStructureIds = new List<int>();
        if (request.StructureId.HasValue && request.StructureId.Value > 0)
        {
            filterStructureIds.Add(request.StructureId.Value);
            GetChildrenStructures(request.StructureId.Value, filterStructureIds);

            // Intersectamos: No puede buscar una red que no le pertenezca
            if (!isSuperAdmin)
            {
                filterStructureIds = filterStructureIds.Intersect(allowedStructureIds).ToList();
                if (!filterStructureIds.Any()) return new List<Dictionary<string, object>>(); 
            }
        }
        else if (!isSuperAdmin)
        {
            filterStructureIds = allowedStructureIds; // Filtro por defecto: todo su árbol
        }

        // Aplicamos el filtro al Query usando los LeaderIds en memoria (Evita errores de EF Core)
        if (filterStructureIds.Any())
        {
            var validLeaderIds = await _context.OrganizationMembers
                .Where(om => filterStructureIds.Contains(om.OrganizationStructureId))
                .Select(om => om.MemberId)
                .Distinct()
                .ToListAsync();

            query = query.Where(x => 
                (x.Event != null && x.Event.OrganizationStructureId.HasValue && filterStructureIds.Contains(x.Event.OrganizationStructureId.Value)) ||
                (x.Event != null && !x.Event.OrganizationStructureId.HasValue && validLeaderIds.Contains(x.LeaderId))
            );
        }

        var rawData = await query.OrderBy(x => x.RegistryDate).ToListAsync();

        // 3. AGRUPAR POR SEMANA EXACTA
        var groupedByWeek = rawData.GroupBy(row => 
        {
            var regDate = row.RegistryDate.ToLocalTime();
            int diff = (7 + (regDate.DayOfWeek - DayOfWeek.Monday)) % 7;
            var startOfWeek = regDate.Date.AddDays(-1 * diff);
            var endOfWeek = startOfWeek.AddDays(6);
            return $"Semana ({startOfWeek:dd/MM} al {endOfWeek:dd/MM})";
        }).ToList();

        var numericColumns = new HashSet<string>();

        // 4. PROCESAR FILAS Y GENERAR SUBTOTALES POR SEMANA
        foreach (var weekGroup in groupedByWeek)
        {
            string weekTag = weekGroup.Key;
            var weekTotals = new Dictionary<string, decimal>();

            foreach (var row in weekGroup)
            {
                var flatRow = new Dictionary<string, object>();
                flatRow["Semana"] = weekTag;
                
                var jsonElements = row.DataJson.RootElement;
                var regDate = row.RegistryDate.ToLocalTime();

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
                            if (element.ValueKind == JsonValueKind.Number)
                            {
                                var val = element.GetDecimal();
                                flatRow[headerLabel] = val;
                                numericColumns.Add(headerLabel);
                                weekTotals[headerLabel] = weekTotals.GetValueOrDefault(headerLabel) + val;
                            }
                            else if (element.ValueKind == JsonValueKind.String && decimal.TryParse(element.GetString(), out var valStr))
                            {
                                flatRow[headerLabel] = valStr;
                                numericColumns.Add(headerLabel);
                                weekTotals[headerLabel] = weekTotals.GetValueOrDefault(headerLabel) + valStr;
                            }
                            else if (element.ValueKind == JsonValueKind.True) flatRow[headerLabel] = "Sí";
                            else if (element.ValueKind == JsonValueKind.False) flatRow[headerLabel] = "No";
                            else flatRow[headerLabel] = element.ToString() ?? "";
                        }
                        else
                        {
                            flatRow[headerLabel] = "";
                        }
                    }
                }
                resultData.Add(flatRow);
            }

            // 🔥 FILA DE SUBTOTAL SEMANAL
            var subtotalRow = new Dictionary<string, object>();
            subtotalRow["Semana"] = $"SUBTOTAL {weekTag}";

            foreach (var colKey in request.SelectedColumns)
            {
                string headerLabel = labelMap.TryGetValue(colKey, out var lbl) ? lbl : colKey;
                if (numericColumns.Contains(headerLabel)) subtotalRow[headerLabel] = weekTotals.GetValueOrDefault(headerLabel, 0);
                else subtotalRow[headerLabel] = "";
            }
            resultData.Add(subtotalRow);
        }

        // 5. FILA DE TOTAL GENERAL
        if (resultData.Count > 0)
        {
            var grandTotalRow = new Dictionary<string, object>();
            grandTotalRow["Semana"] = "TOTAL GENERAL DEL PERÍODO";
            
            foreach (var colKey in request.SelectedColumns)
            {
                string headerLabel = labelMap.TryGetValue(colKey, out var lbl) ? lbl : colKey;
                
                if (numericColumns.Contains(headerLabel))
                {
                    decimal gTotal = 0;
                    foreach (var row in resultData)
                    {
                        if (row["Semana"].ToString()!.StartsWith("SUBTOTAL") && row.TryGetValue(headerLabel, out var val) && val is decimal dVal)
                            gTotal += dVal;
                    }
                    grandTotalRow[headerLabel] = gTotal;
                }
                else grandTotalRow[headerLabel] = "";
            }
            resultData.Add(grandTotalRow);
        }

        return resultData;
    }
}