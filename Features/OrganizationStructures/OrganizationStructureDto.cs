namespace IglesiaBackend.Features.OrganizationStructures;

public record OrganizationStructureDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // 🔥 VITAL: Para el Soft Delete visual
    public bool IsDeleted { get; set; }

    // --- CLASIFICACIÓN ---
    public int OrganizationTypeId { get; set; }
    public string OrganizationTypeName { get; set; } = string.Empty; // Para mostrar "Red", "Ministerio"

    // --- JERARQUÍA ---
    public int? ParentId { get; set; }
    public string? ParentName { get; set; } // Para mostrar "Pertenece a: Red Jóvenes"
}

public class OrganizationStructureCreateUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // ¿Qué es? (ID del OrganizationType)
    public int OrganizationTypeId { get; set; }

    // ¿De quién depende? (Opcional, si es NULL es raíz)
    public int? ParentId { get; set; }
    // 🔥 VITAL: Para el Soft Delete visual
    //public bool IsDeleted { get; set; } = false;
}