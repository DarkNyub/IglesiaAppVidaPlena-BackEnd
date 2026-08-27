using IglesiaBackend.Features.Events;
using IglesiaBackend.Features.RecordTypes;
using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.EventRecordTypes;

/// <summary>
/// Representa la asociación entre un evento y un tipo de registro (Formulario).
/// Permite definir qué tipos de registros están habilitados para un evento específico.
/// </summary>
public class EventRecordType : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int EventId { get; set; }

    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }

    [Required]
    public int RecordTypeId { get; set; }

    [ForeignKey(nameof(RecordTypeId))]
    public RecordType? RecordType { get; set; }
}