using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.OrganizationTypes
{
    /// <summary>
    /// Define el tipo de unidad organizacional (Ej: "Red", "Ministerio", "Grupo de Vida", "Departamento").
    /// Permite clasificar las estructuras.
    /// </summary>
    public class OrganizationType : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Clave interna para lógica de negocio fija (Ej: "NETWORK", "MINISTRY", "LIFE_GROUP").
        /// Útil si el backend necesita diferenciar comportamiento según el tipo.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Key { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }
    }

}
