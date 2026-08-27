using IglesiaBackend.Features.RecordTypes;
using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.RecordTypeFields;

/// <summary>
/// Define un campo dinámico asociado a un tipo de registro.
/// Ejemplo: Nombre, Edad, Fecha de Bautismo, etc.
/// </summary>
public class RecordTypeField : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// identificador del tipo de registro al que pertenece el campo
    /// </summary>
    [Required]
    public int RecordTypeId { get; set; }

    [ForeignKey(nameof(RecordTypeId))]
    public RecordType? RecordType { get; set; }

    /// <summary>
    /// nombre del campo (ej: FirstName, BirthDate)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// tipo de dato del campo (string, int, decimal, bool, date, member_selection)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// Lógica para poblar la lista de miembros si DataType es 'member_selection'.
    /// Ej: "GROUP_MEMBERS" (miembros de mi grupo), "NETWORK_LEADERS" (líderes de mi red).
    /// </summary>
    [MaxLength(50)]
    public string? MemberSelectionLogic { get; set; } // <--- NUEVO

    /// <summary>
    /// indica si el campo es obligatorio
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// orden de visualización del campo
    /// </summary>
    public int FieldOrder { get; set; } = 1;

    /// <summary>
    /// esto es para mapear la propiedad Label a la columna "label" en la base de datos
    /// </summary>
    [Column("label")]
    public string Label { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}