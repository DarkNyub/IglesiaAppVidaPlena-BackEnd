using IglesiaBackend.Features.Events;
using IglesiaBackend.Features.OrganizationMembers;
using IglesiaBackend.Features.OrganizationTypes;
using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.OrganizationStructures
{
    /// <summary>
    /// Representa cualquier unidad organizativa de la iglesia (Nodo del árbol).
    /// Puede ser una Red específica (Red Jóvenes), un Ministerio (Ujieres) o un Grupo de Vida.
    /// </summary>
    public class OrganizationStructure : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        // --- CLASIFICACIÓN ---
        public int OrganizationTypeId { get; set; }

        [ForeignKey(nameof(OrganizationTypeId))]
        public OrganizationType? OrganizationType { get; set; }

        // --- JERARQUÍA RECURSIVA (Composite Pattern) ---

        /// <summary>
        /// ID de la estructura padre. Si es NULL, es una estructura raíz (Ej: Ministerio Principal).
        /// </summary>
        public int? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public OrganizationStructure? Parent { get; set; }

        /// <summary>
        /// Sub-estructuras hijas (Ej: Red -> Grupos de Vida).
        /// </summary>
        public ICollection<OrganizationStructure> Children { get; set; } = new List<OrganizationStructure>();

        // --- RELACIONES ---

        /// <summary>
        /// Miembros asignados a esta estructura con un cargo específico.
        /// </summary>
        public ICollection<OrganizationMember> Members { get; set; } = new List<OrganizationMember>();

        // TODO: Descomentar cuando actualicemos la entidad Event
         public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
