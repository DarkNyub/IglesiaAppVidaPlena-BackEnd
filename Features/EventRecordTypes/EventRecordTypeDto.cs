using System.ComponentModel.DataAnnotations;

namespace IglesiaBackend.Features.EventRecordTypes;

public class EventRecordTypeDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int RecordTypeId { get; set; }
    public string RecordTypeName { get; set; } = string.Empty; // <--- AGREGADO (Útil para mostrar en listas)
}

public class EventRecordTypeCreateUpdateDto
{
    [Required]
    public int EventId { get; set; }

    [Required]
    public int RecordTypeId { get; set; }
}