using IglesiaBackend.Features.RegistryEvents.Dtos;

namespace IglesiaBackend.Features.RegistryEvents;

public static class RegistryEventMapper
{
    public static RegistryEventDto ToDto(RegistryEvent entity)
    {
        return new RegistryEventDto
        {
            Id = entity.Id,
            EventId = entity.EventId,
            EventName = entity.Event?.Name,
            RecordTypeId = entity.RecordTypeId,
            RecordTypeName = entity.RecordType?.Name,
            LeaderId = entity.LeaderId,
            LeaderName = entity.Leader != null ? $"{entity.Leader.FirstName} {entity.Leader.LastName}" : "Desconocido",
            RegistryDate = entity.RegistryDate,
            DataJson = entity.DataJson,

            // 🔥 AGREGAMOS ISDELETED PARA EL FRONTEND
            IsDeleted = entity.IsDeleted
        };
    }

    public static RegistryEvent ToEntity(RegistryEventCreateUpdateDto dto, int leaderId)
    {
        return new RegistryEvent
        {
            EventId = dto.EventId,
            RecordTypeId = dto.RecordTypeId,
            LeaderId = leaderId, // Viene del JWT
            DataJson = dto.DataJson
            // RegistryDate lo setea la BD o el constructor por defecto
        };
    }

    public static void UpdateEntity(RegistryEvent entity, RegistryEventCreateUpdateDto dto)
    {
        entity.EventId = dto.EventId;
        entity.RecordTypeId = dto.RecordTypeId;
        entity.DataJson = dto.DataJson;
    }
}