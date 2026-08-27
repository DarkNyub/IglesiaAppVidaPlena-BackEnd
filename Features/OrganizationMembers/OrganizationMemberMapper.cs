namespace IglesiaBackend.Features.OrganizationMembers;

public static class OrganizationMemberMapper
{
    public static OrganizationMemberDto ToDto(OrganizationMember entity)
    {
        return new OrganizationMemberDto
        {
            Id = entity.Id,

            MemberId = entity.MemberId,
            MemberName = entity.Member != null
                ? $"{entity.Member.FirstName} {entity.Member.LastName}"
                : string.Empty,

            OrganizationStructureId = entity.OrganizationStructureId,
            StructureName = entity.OrganizationStructure?.Name ?? string.Empty,
            StructureTypeName = entity.OrganizationStructure?.OrganizationType?.Name ?? string.Empty,

            ChurchFunctionRoleId = entity.ChurchFunctionRoleId,
            RoleName = entity.ChurchFunctionRole?.Name ?? string.Empty,

            AssignedAt = entity.AssignedAt,
            FinishedAt = entity.FinishedAt,
            IsActive = entity.IsActive,
            IsDeleted = entity.IsDeleted
        };
    }

    public static OrganizationMember ToEntity(OrganizationMemberCreateUpdateDto dto)
    {
        return new OrganizationMember
        {
            MemberId = dto.MemberId,
            OrganizationStructureId = dto.OrganizationStructureId,
            ChurchFunctionRoleId = dto.ChurchFunctionRoleId,
            AssignedAt = dto.AssignedAt,
            FinishedAt = dto.FinishedAt,
            IsActive = dto.IsActive
        };
    }

    public static void UpdateEntity(OrganizationMember entity, OrganizationMemberCreateUpdateDto dto)
    {
        // Nota: Normalmente no se permite cambiar el MemberId ni el StructureId en un update
        // Si se equivocaron, mejor borrar y crear de nuevo. 
        // Pero aquí permitimos editar el ROL o las FECHAS.

        entity.ChurchFunctionRoleId = dto.ChurchFunctionRoleId;
        entity.AssignedAt = dto.AssignedAt;
        entity.FinishedAt = dto.FinishedAt;
        entity.IsActive = dto.IsActive;
    }
}