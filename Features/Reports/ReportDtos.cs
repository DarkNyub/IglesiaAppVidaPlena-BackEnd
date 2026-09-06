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
    public int TypeReport { get; set; }

    [Required]
    public JsonDocument Configuration { get; set; } = JsonDocument.Parse("{}");
}

public class ReportColumnDto
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool IsDynamic { get; set; }
    public bool IsRequired { get; set; }
    public string DataType { get; set; } = "STRING";
}

public class DynamicReportRequestDto
{
    public int RecordTypeId { get; set; }
    public int? EventId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }// 🔥 PROPIEDAD FALTANTE AGREGADA
    public int? StructureId { get; set; }
    public List<string> SelectedColumns { get; set; } = new();
}