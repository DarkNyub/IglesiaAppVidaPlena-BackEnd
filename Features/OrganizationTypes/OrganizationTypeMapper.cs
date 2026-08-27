namespace IglesiaBackend.Features.OrganizationTypes;

public static class OrganizationTypeMapper
{
    public static OrganizationTypeDto ToDto(OrganizationType entity)
    {
        return new OrganizationTypeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Key = entity.Key,
            Description = entity.Description,
            IsDeleted = entity.IsDeleted
        };
    }

    public static OrganizationType ToEntity(OrganizationTypeCreateUpdateDto dto)
    {
        return new OrganizationType
        {
            Name = dto.Name,
            Key = dto.Key, // Importante: Normalmente se guarda en mayúsculas
            Description = dto.Description
        };
    }

    public static void UpdateEntity(OrganizationType entity, OrganizationTypeCreateUpdateDto dto)
    {
        entity.Name = dto.Name;
        entity.Key = dto.Key;
        entity.Description = dto.Description;
        //entity.IsDeleted = dto.IsDeleted;
    }
}