using IglesiaBackend.Features.HierarchyRelations.DTOs;

namespace IglesiaBackend.Features.HierarchyRelations;

public static class HierarchyRelationMapper
{
    public static HierarchyRelationDto ToDto(HierarchyRelation entity)
    {
        return new HierarchyRelationDto
        {
            Id = entity.Id,
            SupervisorId = entity.SupervisorId,
            SubordinateId = entity.SubordinateId,
            CommunityNetworkId = entity.CommunityNetworkId,
            MinistryId = entity.MinistryId
        };
    }

    public static HierarchyRelation ToEntity(HierarchyRelationCreateUpdateDto dto)
    {
        return new HierarchyRelation
        {
            SupervisorId = dto.SupervisorId,
            SubordinateId = dto.SubordinateId,
            CommunityNetworkId = dto.CommunityNetworkId,
            MinistryId = dto.MinistryId
        };
    }

    public static void UpdateEntity(HierarchyRelation entity, HierarchyRelationCreateUpdateDto dto)
    {
        entity.SupervisorId = dto.SupervisorId;
        entity.SubordinateId = dto.SubordinateId;
        entity.CommunityNetworkId = dto.CommunityNetworkId;
        entity.MinistryId = dto.MinistryId;
    }
}
