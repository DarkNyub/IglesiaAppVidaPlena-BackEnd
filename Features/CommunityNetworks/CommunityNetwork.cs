using IglesiaBackend.Features.Members;
using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.CommunityNetworks;

public class CommunityNetwork : AuditableEntity
{
    /// <summary>
    /// identificador para la relación jerárquica de la red de la iglesia
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// nombre de la red de la iglesia
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// por ejemplo, una breve descripción de la red de la iglesia, que es que zona o barrio abarca
    /// </summary>
    [MaxLength(300)]
    public string? Description { get; set; }
    /// <summary>
    /// es para tecnicamente el código único de la red de la iglesia
    /// </summary>
    [MaxLength(50)]
    public string? NetworkCode { get; set; }
    /// <summary>
    /// Leader Id es para referenciar al miembro que es el líder de la red de la iglesia
    /// </summary>
    public int? LeaderId { get; set; }
    [ForeignKey(nameof(LeaderId))]
    public Member? Leader { get; set; }

    // Una red tiene muchos grupos
    public ICollection<LifeGroup> LifeGroups { get; set; } = new List<LifeGroup>();
}
