using System.ComponentModel.DataAnnotations;

namespace IglesiaBackend.Features.OrganizationMembers;

public record OrganizationMemberDto
{
    public int Id { get; set; }

    // --- DATOS DEL MIEMBRO ---
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty; // "Juan Perez"

    // --- DATOS DE LA ESTRUCTURA ---
    public int OrganizationStructureId { get; set; }
    public string StructureName { get; set; } = string.Empty; // "Red Jóvenes"
    public string StructureTypeName { get; set; } = string.Empty; // "Red"

    // --- DATOS DEL ROL ---
    public int ChurchFunctionRoleId { get; set; }
    public string RoleName { get; set; } = string.Empty; // "Líder"

    // --- ESTADO ---
    public DateTime AssignedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public bool IsActive { get; set; }
    // 🔥 AGREGAR ESTO PARA EL FRONTEND
    public bool IsDeleted { get; set; }
}

public class OrganizationMemberCreateUpdateDto
{
    [Required]
    public int MemberId { get; set; }

    [Required]
    public int OrganizationStructureId { get; set; }

    [Required]
    public int ChurchFunctionRoleId { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }
    public bool IsActive { get; set; } = true;
}