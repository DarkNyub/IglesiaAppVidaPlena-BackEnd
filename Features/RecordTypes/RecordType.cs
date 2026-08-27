using IglesiaBackend.Features.ChurchFunctionRoles;
using IglesiaBackend.Features.EventRecordTypes;
using IglesiaBackend.Features.OrganizationTypes;
using IglesiaBackend.Features.RecordTypeFields;
using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.RecordTypes;

/// <summary>
/// Define el tipo de registro que se puede asociar a eventos
/// (ej: Inscripción, Asistencia, Donación, etc.)
/// </summary>
public class RecordType : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }
    
    // --- FILTROS DE VISIBILIDAD (TARGETING) ---

    // A. Por TIPO: "Mostrar a todos los líderes de Grupos de Vida"
    public int? TargetOrganizationTypeId { get; set; }
    public OrganizationType? TargetOrganizationType { get; set; }

    // B. Por ROL: "Mostrar solo a los que tengan el rol de 'Líder' (ID 5)"
    // Si es NULL, se muestra a cualquier miembro de la estructura
    public int? TargetFunctionRoleId { get; set; }
    public ChurchFunctionRole? TargetFunctionRole { get; set; }

    // 🔗 Navegaciones (muy importante)
    public ICollection<RecordTypeField> Fields { get; set; } = new List<RecordTypeField>();
    public ICollection<EventRecordType> EventRecordTypes { get; set; } = new List<EventRecordType>();
}
