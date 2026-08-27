using System.Text.Json;

namespace IglesiaBackend.Features.RegistryEvents.Dtos;

/// <summary>
/// DTO for reading registry events
/// </summary>
public class RegistryEventDto
{
    public int Id { get; set; }

    public int EventId { get; set; }
    public string? EventName { get; set; } // <--- NUEVO

    public int RecordTypeId { get; set; }
    public string? RecordTypeName { get; set; } // <--- NUEVO

    public int LeaderId { get; set; }
    public string? LeaderName { get; set; } // <--- NUEVO

    public DateTime RegistryDate { get; set; }

    /// <summary>
    /// JSON payload with dynamic form data
    /// </summary>
    public JsonDocument? DataJson { get; set; }

    public bool IsDeleted { get; set; }
}

/// <summary>
/// DTO used to create or update a registry event
/// </summary>
public class RegistryEventCreateUpdateDto
{
    public int EventId { get; set; }
    public int RecordTypeId { get; set; }

    /// <summary>
    /// JSON payload containing form data
    /// </summary>
    public JsonDocument DataJson { get; set; } = null!;
}