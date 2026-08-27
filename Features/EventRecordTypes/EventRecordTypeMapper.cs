namespace IglesiaBackend.Features.EventRecordTypes;

public static class EventRecordTypeMapper
{
    public static EventRecordTypeDto ToDto(EventRecordType entity)
    {
        return new EventRecordTypeDto
        {
            Id = entity.Id,
            EventId = entity.EventId,
            RecordTypeId = entity.RecordTypeId,
            // Si el include funcionó, tendremos el nombre, sino string vacío
            RecordTypeName = entity.RecordType?.Name ?? string.Empty
        };
    }

    public static void UpdateEntity(
        EventRecordType entity,
        EventRecordTypeCreateUpdateDto dto)
    {
        entity.EventId = dto.EventId;
        entity.RecordTypeId = dto.RecordTypeId;
    }
}