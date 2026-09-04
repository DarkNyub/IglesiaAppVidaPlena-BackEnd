using IglesiaBackend.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace IglesiaBackend.Features.Events;

public class EventDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public bool IsInPerson { get; set; }

    // Mantenemos IsRecurring por compatibilidad, aunque la lógica fuerte ahora es RecurrenceType
    public bool IsRecurring { get; set; }
    public bool AllowMultipleSubmissionsPerDay { get; set; }

    // --- NUEVA LÓGICA DE RECURRENCIA ---
    public string RecurrenceType { get; set; } = "NONE";
    public int RecurrenceInterval { get; set; } = 1;
    public WeekDays? RecurringDays { get; set; }
    public string EndType { get; set; } = "NEVER";
    public DateTime? EndDate { get; set; }
    public int? MaxOccurrences { get; set; }

    public int? OrganizationStructureId { get; set; }
    public string? OrganizationStructureName { get; set; }

    public List<int> RecordTypeIds { get; set; } = new List<int>();
    public List<string> EvidenceImages { get; set; } = new();
    public bool IsDeleted { get; set; }
}

public class EventCreateUpdateDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsInPerson { get; set; }

    public bool IsRecurring { get; set; }
    public bool AllowMultipleSubmissionsPerDay { get; set; }

    // --- NUEVA LÓGICA DE RECURRENCIA ---
    [Required]
    [MaxLength(20)]
    public string RecurrenceType { get; set; } = "NONE";

    public int RecurrenceInterval { get; set; } = 1;

    public WeekDays? RecurringDays { get; set; }

    [Required]
    [MaxLength(20)]
    public string EndType { get; set; } = "NEVER";

    public DateTime? EndDate { get; set; }

    public int? MaxOccurrences { get; set; }

    public int? OrganizationStructureId { get; set; }

    public List<int> RecordTypeIds { get; set; } = new List<int>();
}