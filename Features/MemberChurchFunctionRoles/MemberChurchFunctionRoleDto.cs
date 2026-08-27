using System.ComponentModel.DataAnnotations;

namespace IglesiaBackend.Features.MemberChurchFunctionRoles;

public class MemberChurchFunctionRoleDto
{
    public int Id { get; set; }

    public int MemberId { get; set; }
    public string? MemberFullName { get; set; }

    public int ChurchFunctionRoleId { get; set; }
    public string? ChurchFunctionRoleName { get; set; }
}

public class MemberChurchFunctionRoleCreateUpdateDto
{
    /// <summary>
    /// Identifier of the member
    /// </summary>
    [Required]
    public int MemberId { get; set; }

    /// <summary>
    /// Identifier of the church function role
    /// </summary>
    [Required]
    public int ChurchFunctionRoleId { get; set; }
}
