using System.ComponentModel.DataAnnotations;

namespace IglesiaBackend.Features.SystemRoles.Dtos;

/// <summary>
/// DTO used for reading system roles
/// </summary>
public class SystemRoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    // 🔥 VITAL: Para el Soft Delete visual
    public bool IsDeleted { get; set; }
}

/// <summary>
/// DTO used to create or update a system role
/// </summary>
public class CreateUpdateSystemRoleDto
{
    [Required(ErrorMessage = "El nombre del rol es obligatorio")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    // 🔥 VITAL: Para el Soft Delete visual
    //public bool IsDeleted { get; set; }
}