using IglesiaBackend.Features.Users.Dtos;
using IglesiaBackend.Features.Users; // Asegura importar la entidad

namespace IglesiaBackend.Features.Users;

public static class UserMapper
{
    public static UserDto ToDto(User entity)
    {
        return new UserDto
        {
            Id = entity.Id,
            Username = entity.Username,
            MemberId = entity.MemberId,
            MemberFullName = entity.Member != null
                ? $"{entity.Member.FirstName} {entity.Member.LastName}"
                : "Desconocido",

            // --- AUDITORÍA Y ESTADO ---
            IsActive = entity.IsActive,
            IsDeleted = entity.IsDeleted,

            // --- ROLES ESTRUCTURADOS ---
            // Filtramos nulls y mapeamos a objeto
            SystemRoles = entity.UserSystemRoles?
                .Where(usr => usr.SystemRole != null && !usr.IsDeleted) // <--- AGREGAR !usr.IsDeleted
                .Select(usr => new UserSystemRoleDto
                {
                    Id = usr.SystemRole!.Id,
                    Name = usr.SystemRole.Name
                })
                .ToList() ?? new List<UserSystemRoleDto>()
        };
    }

    public static User ToEntity(UserCreateUpdateDto dto)
    {
        return new User
        {
            Username = dto.Username,
            PasswordHash = string.Empty, // Se llena en el servicio
            MemberId = dto.MemberId,
            IsActive = dto.IsActive,
            IsDeleted = false // Al crear nace vivo
        };
    }

    public static void UpdateEntity(User entity, UserCreateUpdateDto dto)
    {
        entity.Username = dto.Username;
        entity.MemberId = dto.MemberId;
        entity.IsActive = dto.IsActive;
        // Nota: IsDeleted no se toca aquí, se toca vía Delete o Toggle
    }
}