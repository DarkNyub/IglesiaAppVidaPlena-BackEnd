using IglesiaBackend.Data;
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

    // --- CRUD BÁSICO ---
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
        // En lugar de FindAsync (que no soporta IgnoreQueryFilters fácilmente), usamos FirstOrDefaultAsync
        return await _context.Reports
            .IgnoreQueryFilters() // 🔥 IMPORTANTE: Para poder editar/restaurar uno borrado
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

    // 🔥 AGREGAMOS ESTE MÉTODO PARA LA RESTAURACIÓN
    public async Task UpdateSimpleAsync(Report entity)
    {
        _context.Reports.Update(entity);
        await _context.SaveChangesAsync();
    }

    // =========================================================
    // 1. METADATOS: ¿Qué puedo graficar?
    // =========================================================
    public async Task<ReportMetadataDto> GetReportMetadataAsync()
    {
        var response = new ReportMetadataDto();

        // A. DIMENSIONES (Ejes de Agrupación)
        // En lugar de Reflection ciego, definimos las dimensiones útiles de negocio.
        response.Dimensions.Add(new ReportFieldDto { Key = "registryDate", Label = "Fecha de Registro" });
        response.Dimensions.Add(new ReportFieldDto { Key = "eventName", Label = "Nombre del Evento" });
        response.Dimensions.Add(new ReportFieldDto { Key = "structureName", Label = "Red / Estructura" });
        response.Dimensions.Add(new ReportFieldDto { Key = "leaderName", Label = "Líder Responsable" });
        response.Dimensions.Add(new ReportFieldDto { Key = "recordTypeName", Label = "Tipo de Formulario" });

        // B. MÉTRICAS (Valores Numéricos a Sumar)
        // Buscando en los campos históricos definidos en RecordTypeFields
        var allNumericFields = await _context.RecordTypeFields
            .IgnoreQueryFilters()
            .AsNoTracking()
            // Filtramos por los tipos de datos que definimos en RecordTypeFieldService ("int", "decimal")
            .Where(f => f.DataType == "int" || f.DataType == "decimal")
            .Select(f => new { f.Name, f.Label, f.IsDeleted, f.UpdateAt })
            .ToListAsync();

        // Agrupamos por Key (Name) para no repetir si se borró y creó de nuevo
        var uniqueMetrics = allNumericFields
            .GroupBy(f => f.Name)
            .Select(g =>
            {
                // Preferimos la versión activa, o la más reciente
                var best = g.OrderBy(x => x.IsDeleted).ThenByDescending(x => x.UpdateAt).First();
                return new ReportFieldDto
                {
                    Key = g.Key, // Ej: "offering_amount"
                    Label = best.Label // Ej: "Ofrenda Total"
                };
            })
            .OrderBy(x => x.Label)
            .ToList();

        response.Metrics.AddRange(uniqueMetrics);

        return response;
    }

    // =========================================================
    // 2. GENERACIÓN DE DATOS
    // =========================================================
    public async Task<List<ChartDataDto>> GenerateReportDataAsync(int reportId)
    {
        var report = await _context.Reports.FindAsync(reportId);
        if (report == null) throw new Exception("Reporte no encontrado");

        // 1. Leer Configuración
        ReportConfigDto config;
        try
        {
            var jsonString = report.Configuration.RootElement.GetRawText();
            config = JsonSerializer.Deserialize<ReportConfigDto>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        catch
        {
            return new List<ChartDataDto>(); // Configuración inválida
        }

        if (string.IsNullOrEmpty(config.Dimension) || string.IsNullOrEmpty(config.Metric))
            return new List<ChartDataDto>();

        // 2. Traer Datos (Incluyendo relaciones para navegar)
        // Nota: Para grandes volúmenes, esto debería filtrarse por fecha en la BD primero.
        var query = _context.RegistryEvents
            .AsNoTracking()
            .Include(x => x.Event).ThenInclude(e => e.OrganizationStructure) // Para nombre de Red
            .Include(x => x.Leader) // Para nombre de Líder
            .Include(x => x.RecordType) // Para nombre de Formulario
            .AsQueryable();

        // (Opcional) Si el config tuviera fechas, filtraríamos aquí:
        // if (config.StartDate.HasValue) query = query.Where(x => x.RegistryDate >= config.StartDate);

        var rawData = await query.ToListAsync();

        // 3. Agrupar en Memoria (Strategy Pattern simple)
        var groupedData = rawData.GroupBy(item => GetDimensionValue(item, config.Dimension));

        // 4. Sumar JSON y Proyectar
        var chartData = groupedData.Select(g => new ChartDataDto
        {
            Label = g.Key,
            Value = g.Sum(item => ExtractMetricValue(item, config.Metric))
        })
        .Where(x => x.Value > 0) // Opcional: Ocultar ceros
        .OrderByDescending(x => x.Value) // Opcional: Ordenar por valor
        .ToList();

        return chartData;
    }

    // Helper: Obtener el valor de la dimensión (Eje X)
    private string GetDimensionValue(RegistryEvent item, string dimensionKey)
    {
        return dimensionKey switch
        {
            "registryDate" => item.RegistryDate.ToLocalTime().ToString("yyyy-MM-dd"), // Agrupar por día
            "eventName" => item.Event?.Name ?? "Evento Eliminado",
            "structureName" => item.Event?.OrganizationStructure?.Name ?? "Sin Estructura",
            "leaderName" => item.Leader != null ? $"{item.Leader.FirstName} {item.Leader.LastName}" : "Sin Líder",
            "recordTypeName" => item.RecordType?.Name ?? "Desconocido",
            _ => "Dimensión Desconocida"
        };
    }

    // Helper: Extraer valor numérico del JSON (Eje Y)
    private double ExtractMetricValue(RegistryEvent item, string metricKey)
    {
        try
        {
            // Buscamos la clave (ej: "offering_amount") en la raíz del JSON
            if (item.DataJson.RootElement.TryGetProperty(metricKey, out var element))
            {
                if (element.ValueKind == JsonValueKind.Number)
                {
                    return element.GetDouble();
                }
                // Soporte para strings numéricos (ej: "150.50")
                else if (element.ValueKind == JsonValueKind.String && double.TryParse(element.GetString(), out var val))
                {
                    return val;
                }
            }
        }
        catch { /* Ignorar errores de parseo individual */ }

        return 0.0;
    }
    // =========================================================
    // 1. OBTENER COLUMNAS DISPONIBLES (El Menú para el Usuario)
    // =========================================================
    public async Task<List<ReportColumnDto>> GetAvailableColumnsAsync(int recordTypeId)
    {
        var columns = new List<ReportColumnDto>
        {
            // A. Columnas Fijas (Siempre están en la bitácora)
            new ReportColumnDto { Key = "registryDate", Label = "Fecha de Registro", IsDynamic = false },
            new ReportColumnDto { Key = "eventName", Label = "Evento", IsDynamic = false },
            new ReportColumnDto { Key = "structureName", Label = "Red / Estructura", IsDynamic = false },
            new ReportColumnDto { Key = "leaderName", Label = "Líder Reporta", IsDynamic = false }
        };

        // B. Columnas Dinámicas (Las que crearon en el formulario)
        var dynamicFields = await _context.RecordTypeFields
            .IgnoreQueryFilters() // Traemos todo por si hay data histórica
            .AsNoTracking()
            .Where(f => f.RecordTypeId == recordTypeId
                     // Solo tipos de datos que sirvan para reportes planos
                     && (f.DataType == "int" || f.DataType == "decimal" || f.DataType == "bool" || f.DataType == "date" || f.DataType == "text"))
            .OrderBy(f => f.FieldOrder)
            .Select(f => new ReportColumnDto { Key = f.Name, Label = f.Label, IsDynamic = true })
            .ToListAsync();

        // Evitamos duplicados si borraron y recrearon un campo con el mismo Key
        var uniqueDynamicFields = dynamicFields.GroupBy(f => f.Key).Select(g => g.First()).ToList();

        columns.AddRange(uniqueDynamicFields);
        return columns;
    }

    // =========================================================
    // 2. GENERADOR DEL REPORTE PLANO (La Tabla Dinámica)
    // =========================================================
    public async Task<List<Dictionary<string, object>>> GenerateFlatReportAsync(DynamicReportRequestDto request)
    {
        var resultData = new List<Dictionary<string, object>>();

        // 1. Armar la consulta base a la Bitácora
        var query = _context.RegistryEvents
            .AsNoTracking()
            .IgnoreQueryFilters() // Para reportes queremos ver todo
            .Include(x => x.Event).ThenInclude(e => e.OrganizationStructure)
            .Include(x => x.Leader)
            .Where(x => x.RecordTypeId == request.RecordTypeId);

        // 2. Aplicar Filtros Dinámicos
        if (request.StartDate.HasValue)
            query = query.Where(x => x.RegistryDate >= request.StartDate.Value);

        if (request.EndDate.HasValue)
        {
            var endOfDay = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(x => x.RegistryDate <= endOfDay);
        }

        if (request.StructureId.HasValue && request.StructureId.Value > 0)
            query = query.Where(x => x.Event != null && x.Event.OrganizationStructureId == request.StructureId.Value);

        // Traemos la data cruda
        var rawData = await query.OrderByDescending(x => x.RegistryDate).ToListAsync();

        // 3. Proyectar solo las columnas seleccionadas
        foreach (var row in rawData)
        {
            var flatRow = new Dictionary<string, object>();
            var jsonElements = row.DataJson.RootElement;

            foreach (var colKey in request.SelectedColumns)
            {
                // Mapear columnas fijas
                if (colKey == "registryDate") flatRow[colKey] = row.RegistryDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
                else if (colKey == "eventName") flatRow[colKey] = row.Event?.Name ?? "N/A";
                else if (colKey == "structureName") flatRow[colKey] = row.Event?.OrganizationStructure?.Name ?? "General";
                else if (colKey == "leaderName") flatRow[colKey] = row.Leader != null ? $"{row.Leader.FirstName} {row.Leader.LastName}" : "N/A";

                // Mapear columnas dinámicas (Buscar en el JSON)
                else
                {
                    if (jsonElements.TryGetProperty(colKey, out var element))
                    {
                        flatRow[colKey] = element.ValueKind switch
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
                        flatRow[colKey] = ""; // Celda vacía si el líder no llenó ese campo
                    }
                }
            }
            resultData.Add(flatRow);
        }

        return resultData;
    }
}