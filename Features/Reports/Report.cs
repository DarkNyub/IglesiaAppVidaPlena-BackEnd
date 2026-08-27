using IglesiaBackend.Shared.Entities; // Asumo que heredas de AuditableEntity
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace IglesiaBackend.Features.Reports;

public class Report : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    /// <summary>
    /// 1=Bar, 2=Pie, 3=Line, 4=Table
    /// </summary>
    [Required]
    public int TypeReport { get; set; }

    /// <summary>
    /// Almacena la configuración dinámica del reporte (ejes, métricas, filtros)
    /// </summary>
    [Required]
    public JsonDocument Configuration { get; set; } = JsonDocument.Parse("{}");
}