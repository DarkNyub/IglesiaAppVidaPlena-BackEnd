using IglesiaBackend.Features.ChurchFunctionRoles;
using IglesiaBackend.Features.Members;
using IglesiaBackend.Features.OrganizationStructures;
using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.OrganizationMembers
{
    /// <summary>
    /// Tabla intermedia que define la participación de un miembro en una estructura.
    /// Responde a: "¿Qué hace Juan en la Red de Jóvenes?" -> "Es Líder".
    /// </summary>
    public class OrganizationMember : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // --- MIEMBRO ---
        public int MemberId { get; set; }

        [ForeignKey(nameof(MemberId))]
        public Member? Member { get; set; }

        // --- ESTRUCTURA ---
        public int OrganizationStructureId { get; set; }

        [ForeignKey(nameof(OrganizationStructureId))]
        public OrganizationStructure? OrganizationStructure { get; set; }

        // --- ROL / FUNCIÓN ---
        public int ChurchFunctionRoleId { get; set; }

        [ForeignKey(nameof(ChurchFunctionRoleId))]
        public ChurchFunctionRole? ChurchFunctionRole { get; set; }

        // --- HISTORIAL Y ESTADO ---

        [Required]
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public DateTime? FinishedAt { get; set; }

        /// <summary>
        /// Indica si el miembro sigue activo en este cargo dentro de esta estructura.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
