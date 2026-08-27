using IglesiaBackend.Features.EventRecordTypes;
using IglesiaBackend.Features.OrganizationStructures;
using IglesiaBackend.Shared.Entities;
using IglesiaBackend.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.Events;

/// <summary>
/// la idea de esta tabla es tener eventos generales de la iglesia
/// sin embargo, estos eventos pueden tener tipos de registros asociados, que van en otra tabla
/// </summary>
public class Event : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// nombre del evento
    /// </summary>
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// fecha principal del evento (para eventos no recurrentes)
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>
    /// descripción opcional del evento
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// indica si el evento es presencial o virtual
    /// </summary>
    public bool IsInPerson { get; set; } = false;

    /// <summary>
    /// indica si el evento es recurrente
    /// </summary>
    public bool IsRecurring { get; set; } = false;

    /// <summary>
    /// Tipo de recurrencia: "NONE", "DAILY", "WEEKLY", "MONTHLY", "ANNUALLY"
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string RecurrenceType { get; set; } = "NONE";

    /// <summary>
    /// Frecuencia. Ej: Cada (1) semana, Cada (2) meses.
    /// </summary>
    public int RecurrenceInterval { get; set; } = 1;

    /// <summary>
    /// Días de la semana en bitmask (L=1, M=2, X=4...). Solo si RecurrenceType = WEEKLY
    /// </summary>
    public WeekDays? RecurringDays { get; set; }

    /// <summary>
    /// Cómo termina: "NEVER", "UNTIL_DATE", "AFTER_OCCURRENCES"
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string EndType { get; set; } = "NEVER";

    /// <summary>
    /// Fecha de finalización si EndType = "UNTIL_DATE"
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Límite de repeticiones si EndType = "AFTER_OCCURRENCES"
    /// </summary>
    public int? MaxOccurrences { get; set; }

    // --- PROPIETARIO DEL EVENTO (NUEVO) ---
    // Si es NULL, es un evento Global de la Iglesia
    public int? OrganizationStructureId { get; set; }

    [ForeignKey(nameof(OrganizationStructureId))]
    public OrganizationStructure? OrganizationStructure { get; set; }

    // Formularios activos para este evento
    public ICollection<EventRecordType> EventRecordTypes { get; set; } = new List<EventRecordType>();
    
    // LISTA DE IMÁGENES (URLs o Base64)
    // Se guardará como JSONB en Postgres
    //public List<string> EvidenceImages { get; set; } = new List<string>();
}