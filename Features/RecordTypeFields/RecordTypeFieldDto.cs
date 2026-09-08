using System.ComponentModel.DataAnnotations;

namespace IglesiaBackend.Features.RecordTypeFields;

/// <summary>
/// DTO de salida para campos de tipo de registro
/// </summary>
public class RecordTypeFieldDto
{
    public int Id { get; set; }
    public int RecordTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string? MemberSelectionLogic { get; set; } // <--- NUEVO
    public bool IsFormula { get; set; } = false;
    public string? FormulaExpression { get; set; }
    public bool IsRequired { get; set; }
    public int FieldOrder { get; set; }
    // 🔥 AGREGAR PARA VISIBILIDAD
    public bool IsDeleted { get; set; }
}

/// <summary>
/// DTO para crear o actualizar campos de tipo de registro
/// </summary>
public class RecordTypeFieldCreateUpdateDto
{
    [Required]
    public int RecordTypeId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// string, int, decimal, bool, date, member_selection
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string DataType { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? MemberSelectionLogic { get; set; } // <--- NUEVO
    public bool IsFormula { get; set; } = false;
    public string? FormulaExpression { get; set; }

    public bool IsRequired { get; set; } = false;

    public int FieldOrder { get; set; } = 1;
}