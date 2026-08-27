using IglesiaBackend.Features.Events;
using IglesiaBackend.Features.Members;
using IglesiaBackend.Features.RecordTypes;
using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace IglesiaBackend.Features.RegistryEvents;

/// <summary>
/// Represents a registry entry for an event, storing captured form data in JSON.
/// </summary>
public class RegistryEvent : AuditableEntity
{
    /// <summary>
    /// Primary key (serial/identity).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Event reference.
    /// </summary>
    [Required]
    public int EventId { get; set; }

    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }

    /// <summary>
    /// Record type reference (defines the form fields).
    /// </summary>
    [Required]
    public int RecordTypeId { get; set; }

    [ForeignKey(nameof(RecordTypeId))]
    public RecordType? RecordType { get; set; }

    /// <summary>
    /// Leader who performed the registry.
    /// This value comes from JWT (UserId/MemberId).
    /// </summary>
    [Required]
    public int LeaderId { get; set; }

    [ForeignKey(nameof(LeaderId))]
    public Member? Leader { get; set; }

    /// <summary>
    /// Registry date (UTC).
    /// </summary>
    public DateTime RegistryDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// JSON data representing dynamic form fields and values.
    /// EF Core con Npgsql mapeará esto directamente a la columna jsonb
    /// </summary>
    [Required]
    public JsonDocument DataJson { get; set; } = null!;
}