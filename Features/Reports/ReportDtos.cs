using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace IglesiaBackend.Features.Reports.Dtos;

public class ReportDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TypeReport { get; set; }
    public JsonDocument Configuration { get; set; } = JsonDocument.Parse("{}");
    public bool IsDeleted { get; set; }
}

public class ReportCreateUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    [Required]
    public int TypeReport { get; set; } // 1=Bar, 2=Pie, 3=Line, 4=Table

    [Required]
    public JsonDocument Configuration { get; set; } = JsonDocument.Parse("{}");
}

// Clase helper para deserializar el JSON de configuración
public class ReportConfigDto
{
    public string Dimension { get; set; } = string.Empty; // Eje X (Ej: "structureName", "registryDate")
    public string Metric { get; set; } = string.Empty;    // Eje Y (Ej: "offering_amount")

    // Filtros opcionales que podrías agregar en el futuro
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class ChartDataDto
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
}

public class ReportFieldDto
{
    public string Key { get; set; } = string.Empty;   // Lo que guardamos en BD
    public string Label { get; set; } = string.Empty; // Lo que mostramos al usuario
}

public class ReportMetadataDto
{
    public List<ReportFieldDto> Dimensions { get; set; } = new();
    public List<ReportFieldDto> Metrics { get; set; } = new();
}

// 1. DTO para decirle a Flutter qué columnas existen
public class ReportColumnDto
{
    public string Key { get; set; } = string.Empty;   // Ej: "registryDate" o "nuevos_integrantes"
    public string Label { get; set; } = string.Empty; // Ej: "Fecha de Registro" o "Nuevos Integrantes"
    public bool IsDynamic { get; set; }               // Para saber si viene del JSON o es fija
}

// 2. DTO de lo que Flutter nos manda para generar la tabla
public class DynamicReportRequestDto
{
    public int RecordTypeId { get; set; } // ¿Qué formulario vamos a exportar?

    // Filtros
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? StructureId { get; set; } // Por si quiere filtrar solo "Red Jóvenes"

    // Las columnas que el usuario marcó con Checkbox
    public List<string> SelectedColumns { get; set; } = new();
}