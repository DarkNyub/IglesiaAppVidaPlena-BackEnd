using IglesiaBackend.Features.Members;
using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.Ministries;

public class Ministry : AuditableEntity
{
    /// <summary>
    /// identidicador para la relacion del ministerio
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    /// <summary>
    /// nombre del ministerio
    /// </summary>
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// breve descripcion de lo que hace el ministerio
    /// </summary>
    [MaxLength(400)]
    public string? Description { get; set; }
    /// <summary>
    /// identificador del líder del ministerio
    /// </summary>
    public int? LeaderId { get; set; }
    [ForeignKey(nameof(LeaderId))]
    public Member? Leader { get; set; }
}
