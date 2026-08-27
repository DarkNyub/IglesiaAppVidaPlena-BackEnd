using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.ChurchFunctionRoles;

/// <summary>
/// Catálogo de cargos o funciones dentro de la iglesia (Ej: "Líder", "Coordinador", "Servidor", "Tesorero").
/// Se asignan a un miembro dentro de una estructura específica.
/// </summary>
public class ChurchFunctionRole : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    /// <summary>
    /// para definir el nombre del rol o función dentro de la iglesia
    /// </summary>
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Nivel de autoridad para ordenamiento jerárquico visual.
    /// Ej: 100 = Pastor, 80 = Coordinador, 50 = Líder, 10 = Miembro.
    /// </summary>
    public int AuthorityLevel { get; set; } = 0;
}