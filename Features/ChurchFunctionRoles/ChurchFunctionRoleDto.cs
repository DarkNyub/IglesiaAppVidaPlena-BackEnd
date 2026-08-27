using System.ComponentModel.DataAnnotations;

namespace IglesiaBackend.Features.ChurchFunctionRoles;

public class ChurchFunctionRoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int AuthorityLevel { get; set; } // <--- AGREGADO// Estado
    public bool IsDeleted { get; set; }
}

public class ChurchFunctionRoleCreateUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }

    public int AuthorityLevel { get; set; } = 0; // <--- AGREGADO
}