using IglesiaBackend.Features.RecordTypeFields;

namespace IglesiaBackend.Features.RecordTypes.Dtos;

public class RecordTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // 🔥 VITAL: Para el Soft Delete visual
    public bool IsDeleted { get; set; }

    // --- TARGETING ---
    public int? TargetOrganizationTypeId { get; set; }
    public string? TargetOrganizationTypeName { get; set; } // Para mostrar nombre

    public int? TargetFunctionRoleId { get; set; }
    public string? TargetFunctionRoleName { get; set; } // Para mostrar nombre

    public List<RecordTypeFieldDto> Fields { get; set; } = new();
}

public class RecordTypeCreateUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    // 🔥 VITAL: Para el Soft Delete visual
    //public bool IsDeleted { get; set; }

    // --- TARGETING ---
    public int? TargetOrganizationTypeId { get; set; }
    public int? TargetFunctionRoleId { get; set; }

    public List<RecordTypeFieldCreateUpdateDto> Fields { get; set; } = new();
}

public class FormStructureDto
{
    public int RecordTypeId { get; set; }
    public string FormName { get; set; } = string.Empty;
    public List<RecordTypeFieldDto> Fields { get; set; } = new();
    // 🔥 VITAL: Para el Soft Delete visual
    public bool IsDeleted { get; set; }
}