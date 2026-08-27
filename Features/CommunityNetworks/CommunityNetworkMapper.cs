namespace IglesiaBackend.Features.CommunityNetworks;

public static class CommunityNetworkMapper
{
    public static CommunityNetworkDto ToDto(CommunityNetwork entity)
    {
        return new CommunityNetworkDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            NetworkCode = entity.NetworkCode,
            LeaderId = entity.LeaderId,
            LeaderName = entity.Leader != null
                ? $"{entity.Leader.FirstName} {entity.Leader.LastName}"
                : null
        };
    }

    public static void MapCreateUpdateDto(
        CommunityNetworkCreateUpdateDto dto,
        CommunityNetwork entity)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.NetworkCode = dto.NetworkCode;
        entity.LeaderId = dto.LeaderId;
    }
}
