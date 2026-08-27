using IglesiaBackend.Features.ChurchFunctionRoles;
using IglesiaBackend.Features.Members;
using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.MemberChurchFunctionRoles;

/// <summary>
/// Reference entity that represents the assignment of a church function role
/// to a member. A member can have one or multiple church function roles assigned.
/// </summary>
public class MemberChurchFunctionRole : AuditableEntity
{
    /// <summary>
    /// Primary key for the member–church function role relationship
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Identifier of the member to whom the role is assigned
    /// </summary>
    [Required]
    public int MemberId { get; set; }

    [ForeignKey(nameof(MemberId))]
    public Member? Member { get; set; }

    /// <summary>
    /// Identifier of the church function role assigned to the member
    /// </summary>
    [Required]
    public int ChurchFunctionRoleId { get; set; }

    [ForeignKey(nameof(ChurchFunctionRoleId))]
    public ChurchFunctionRole? ChurchFunctionRole { get; set; }
}
