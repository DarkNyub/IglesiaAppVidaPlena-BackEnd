namespace IglesiaBackend.Features.ChurchFunctionRoles;

public static class ChurchFunctionRoleMapper
{
    public static ChurchFunctionRoleDto ToDto(ChurchFunctionRole entity)
    {
        return new ChurchFunctionRoleDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            AuthorityLevel = entity.AuthorityLevel, // <--- AGREGADO
            IsDeleted = entity.IsDeleted
        };
    }

    public static void MapCreateUpdateDto(
        ChurchFunctionRoleCreateUpdateDto dto,
        ChurchFunctionRole entity)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.AuthorityLevel = dto.AuthorityLevel; // <--- AGREGADO
    }
}