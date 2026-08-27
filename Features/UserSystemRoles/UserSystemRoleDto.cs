namespace IglesiaBackend.Features.UserSystemRoles.DTOs;

/// <summary>
/// Read DTO
/// </summary>
public class UserSystemRolDto
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    public int SystemRoleId { get; set; }
    public string SystemRoleName { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}

/// <summary>
/// Create / Update DTO
/// </summary>
public class UserSystemRolCreateUpdateDto
{
    public int UserId { get; set; }
    public int SystemRoleId { get; set; }
}
