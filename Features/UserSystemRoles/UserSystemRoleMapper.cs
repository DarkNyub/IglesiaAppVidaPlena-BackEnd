using IglesiaBackend.Features.UserSystemRoles.DTOs;

namespace IglesiaBackend.Features.UserSystemRoles;

public static class UserSystemRoleMapper
{
    public static UserSystemRolDto ToDto(UserSystemRole entity)
    {
        return new UserSystemRolDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            UserName = entity.User?.Username ?? string.Empty,
            SystemRoleId = entity.SystemRoleId,
            SystemRoleName = entity.SystemRole?.Name ?? string.Empty,

            // 🔥 AGREGAR MAPEO
            IsDeleted = entity.IsDeleted
        };
    }
}
