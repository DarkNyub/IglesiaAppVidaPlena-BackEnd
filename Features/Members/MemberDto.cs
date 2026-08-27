using System.Text.Json;

namespace IglesiaBackend.Features.Members;

public record MemberDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Document { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public JsonDocument? ExtraData { get; set; }

    // --- AUDITORÍA Y ESTADO (Heredados de AuditableEntity) ---
    public bool IsDeleted { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }

    // --- USUARIO VINCULADO ---
    // En lugar de solo un bool, devolvemos el objeto pequeño para hacer el link
    public MemberLinkedUserDto? LinkedUser { get; set; }
    // Mantenemos el bool por comodidad visual, calculado si LinkedUser no es null
    public bool HasUserAccount => LinkedUser != null;

    // --- ROLES ---
    // Detalles completos (ID + Nombre) para el Formulario (GetById)
    public List<MemberRoleDto> Roles { get; set; } = new();
    // Resumen de nombres para la Tabla (GetAll)
    public List<string> RolesSummary { get; set; } = new List<string>();
}

// DTO pequeño para info del usuario vinculado
public class MemberLinkedUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
}

public class MemberRoleDto
{
    public int OrganizationStructureId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public int ChurchFunctionRoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
}

// ... El MemberCreateUpdateDto se mantiene igual o con ajustes mínimos ...
public class MemberCreateUpdateDto
{
    // ... tus campos existentes ...
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Document { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    // CAMBIO: Opcional, usualmente al crear es false, pero al editar podemos querer reactivarlo (IsDeleted = false)
    //public bool IsDeleted { get; set; } = false;
    public JsonDocument? ExtraData { get; set; }
    public List<MemberRoleAssignmentDto> Roles { get; set; } = new();

    // NOTA: No enviamos CreatedBy/Date desde el front, eso lo maneja el Backend automáticamente.
}

public class MemberRoleAssignmentDto
{
    public int StructureId { get; set; }
    public int RoleId { get; set; }
}
public class MemberBulkDto
{
    public List<MemberCreateUpdateDto> Members { get; set; } = new();
}