namespace IglesiaBackend.Features.OrganizationTypes;

public record OrganizationTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    // 🔥 VITAL: Para el Soft Delete visual
    public bool IsDeleted { get; set; }
}

public class OrganizationTypeCreateUpdateDto
{
    // Validaciones básicas se pueden agregar con DataAnnotations aquí
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    // 🔥 VITAL: Para el Soft Delete visual
    //public bool IsDeleted { get; set; }
}