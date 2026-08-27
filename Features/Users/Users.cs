using IglesiaBackend.Features.Members;
using IglesiaBackend.Features.UserSystemRoles;
using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.Users;

/// <summary>
/// Represents a system user.
/// A user is linked to a Member and has SystemRoles for authorization.
/// </summary>
public class User : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Username used for authentication (usually Email)
    /// </summary>
    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password hash (never store plain passwords)
    /// </summary>
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Associated church member
    /// </summary>
    [Required]
    public int MemberId { get; set; }

    [ForeignKey(nameof(MemberId))]
    // 🔥 ESTA ES LA CLAVE: Le dice "Este Member se conecta con la propiedad 'User' del otro lado"
    [InverseProperty("User")]
    public Member? Member { get; set; }

    /// <summary>
    /// Indicates whether the user is active (can login)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// System roles assigned to the user
    /// </summary>
    public ICollection<UserSystemRole> UserSystemRoles { get; set; }
        = new List<UserSystemRole>();
}