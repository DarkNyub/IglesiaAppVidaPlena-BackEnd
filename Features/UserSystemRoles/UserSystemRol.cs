using IglesiaBackend.Features.SystemRoles;
using IglesiaBackend.Features.Users;
using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.UserSystemRoles;

/// <summary>
/// Relation between User and SystemRole
/// </summary>
public class UserSystemRole : AuditableEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// User identifier (FK)
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// User navigation property
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    /// <summary>
    /// System role identifier (FK)
    /// </summary>
    [Required]
    public int SystemRoleId { get; set; }

    /// <summary>
    /// System role navigation property
    /// </summary>
    [ForeignKey(nameof(SystemRoleId))]
    public SystemRole SystemRole { get; set; } = null!;
}
