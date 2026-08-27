using System.Text.Json;

namespace IglesiaBackend.Features.Users.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;

    public int MemberId { get; set; }
    public string MemberFullName { get; set; } = string.Empty;

    // --- ESTADO Y AUDITORÍA ---
    public bool IsActive { get; set; } // ¿Puede loguearse?
    public bool IsDeleted { get; set; } // ¿Está en la papelera?

    // --- ROLES DE SISTEMA (Estructurado) ---
    public List<UserSystemRoleDto> SystemRoles { get; set; } = new();

    // Helper para mostrar nombres en la tabla simple (opcional pero útil)
    public string RolesSummary => string.Join(", ", SystemRoles.Select(x => x.Name));
}

public class UserSystemRoleDto
{
    public int Id { get; set; } // ID del SystemRole (ej. 1 para Admin)
    public string Name { get; set; } = string.Empty; // Nombre (ej. "SuperAdmin")
}

public class UserCreateUpdateDto
{
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public int MemberId { get; set; }
    public bool IsActive { get; set; } = true;

    // IDs de los roles seleccionados en el front (ej. [1, 2])
    public List<int> SystemRoleIds { get; set; } = new List<int>();
}