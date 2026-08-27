using IglesiaBackend.Features.UserSystemRoles;
using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.SystemRoles;

/// <summary>
/// Defines system-level roles used for authorization and access control
/// (e.g. Admin, User, Reports, SuperAdmin).
/// These roles are NOT church functions.
/// </summary>
public class SystemRole : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Unique name of the system role
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description explaining the permissions of the role
    /// </summary>
    [MaxLength(250)]
    public string? Description { get; set; }

    /// <summary>
    /// Users assigned to this system role
    /// </summary>
    public ICollection<UserSystemRole> UserSystemRoles { get; set; }
        = new List<UserSystemRole>();
}