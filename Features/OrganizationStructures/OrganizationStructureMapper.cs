namespace IglesiaBackend.Features.OrganizationStructures;

public static class OrganizationStructureMapper
{
    public static OrganizationStructureDto ToDto(OrganizationStructure entity)
    {
        return new OrganizationStructureDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsDeleted = entity.IsDeleted,

            OrganizationTypeId = entity.OrganizationTypeId,
            OrganizationTypeName = entity.OrganizationType?.Name ?? string.Empty,

            ParentId = entity.ParentId,
            ParentName = entity.Parent?.Name // Puede ser null si es raíz
        };
    }

    public static OrganizationStructure ToEntity(OrganizationStructureCreateUpdateDto dto)
    {
        return new OrganizationStructure
        {
            Name = dto.Name,
            Description = dto.Description,
            OrganizationTypeId = dto.OrganizationTypeId,
            ParentId = dto.ParentId
        };
    }

    public static void UpdateEntity(OrganizationStructure entity, OrganizationStructureCreateUpdateDto dto)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.OrganizationTypeId = dto.OrganizationTypeId;
        entity.ParentId = dto.ParentId;
    }
}