
namespace IglesiaBackend.Features.Events;

public static class EventMapper
{
    public static EventDto ToDto(Event entity)
    {
        return new EventDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Date = entity.Date,
            Description = entity.Description,
            IsInPerson = entity.IsInPerson,
            IsRecurring = entity.IsRecurring,

            // --- NUEVOS CAMPOS ---
            RecurrenceType = entity.RecurrenceType,
            RecurrenceInterval = entity.RecurrenceInterval,
            RecurringDays = entity.RecurringDays,
            AllowMultipleSubmissionsPerDay = entity.AllowMultipleSubmissionsPerDay, // En el ToDto
            EndType = entity.EndType,
            EndDate = entity.EndDate,
            MaxOccurrences = entity.MaxOccurrences,

            IsDeleted = entity.IsDeleted,

            OrganizationStructureId = entity.OrganizationStructureId,
            OrganizationStructureName = entity.OrganizationStructure?.Name,

            RecordTypeIds = entity.EventRecordTypes?
                                  .Where(x => !x.IsDeleted)
                                  .Select(x => x.RecordTypeId)
                                  .ToList() ?? new List<int>(),
        };
    }

    public static Event ToEntity(EventCreateUpdateDto dto)
    {
        return new Event
        {
            Name = dto.Name,
            Date = dto.Date,
            Description = dto.Description,
            IsInPerson = dto.IsInPerson,

            // Si el tipo no es NONE, entonces es recurrente.
            IsRecurring = dto.RecurrenceType != "NONE",
            AllowMultipleSubmissionsPerDay = dto.AllowMultipleSubmissionsPerDay, // En el ToEntity / Update

            // --- NUEVOS CAMPOS ---
            RecurrenceType = dto.RecurrenceType,
            RecurrenceInterval = dto.RecurrenceInterval,
            RecurringDays = dto.RecurrenceType == "WEEKLY" ? dto.RecurringDays : null,
            EndType = dto.EndType,
            EndDate = dto.EndType == "UNTIL_DATE" ? dto.EndDate : null,
            MaxOccurrences = dto.EndType == "AFTER_OCCURRENCES" ? dto.MaxOccurrences : null,

            OrganizationStructureId = dto.OrganizationStructureId
        };
    }

    public static void UpdateEntity(Event entity, EventCreateUpdateDto dto)
    {
        entity.Name = dto.Name;
        entity.Date = dto.Date;
        entity.Description = dto.Description;
        entity.IsInPerson = dto.IsInPerson;

        entity.IsRecurring = dto.RecurrenceType != "NONE";

        // --- NUEVOS CAMPOS ---
        entity.RecurrenceType = dto.RecurrenceType;
        entity.RecurrenceInterval = dto.RecurrenceInterval;
        entity.AllowMultipleSubmissionsPerDay = dto.AllowMultipleSubmissionsPerDay; // En el ToEntity / Update
        entity.RecurringDays = dto.RecurrenceType == "WEEKLY" ? dto.RecurringDays : null;
        entity.EndType = dto.EndType;
        entity.EndDate = dto.EndType == "UNTIL_DATE" ? dto.EndDate : null;
        entity.MaxOccurrences = dto.EndType == "AFTER_OCCURRENCES" ? dto.MaxOccurrences : null;

        entity.OrganizationStructureId = dto.OrganizationStructureId;
    }
}