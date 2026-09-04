using IglesiaBackend.Features.SystemRoles.Dtos;


namespace IglesiaBackend.Features.SystemRoles;

public static class SystemRoleMapper
{
    public static SystemRoleDto ToDto(SystemRole entity)
    {
        return new SystemRoleDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsDeleted = entity.IsDeleted
        };
    }

    public static SystemRole ToEntity(CreateUpdateSystemRoleDto dto)
    {
        return new SystemRole
        {
            Name = dto.Name,
            Description = dto.Description
        };
    }

    public static void UpdateEntity(SystemRole entity, CreateUpdateSystemRoleDto dto)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        //entity.IsDeleted = dto.IsDeleted;
    }
}