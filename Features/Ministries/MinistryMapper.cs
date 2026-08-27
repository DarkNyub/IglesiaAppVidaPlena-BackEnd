namespace IglesiaBackend.Features.Ministries;

public static class MinistryMapper
{
    public static MinistryDto ToDto(Ministry entity)
    {
        return new MinistryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            LeaderId = entity.LeaderId,
            LeaderName = entity.Leader != null
                ? $"{entity.Leader.FirstName} {entity.Leader.LastName}"
                : null
        };
    }

    public static void MapCreateUpdateDto(
        MinistryCreateUpdateDto dto,
        Ministry entity)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.LeaderId = dto.LeaderId;
    }
}
