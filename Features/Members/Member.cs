using IglesiaBackend.Features.OrganizationMembers;
using IglesiaBackend.Features.Users;
using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace IglesiaBackend.Features.Members;

/// <summary>
/// Member entity. Id is serial/identity in DB (auto-increment).
/// </summary>
public class Member : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Document { get; set; }

    [MaxLength(30)]
    public string? Phone { get; set; }

    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(2000)]
    public string? Address { get; set; }

    public DateTime? BirthDate { get; set; }

    /// <summary>JSON optional for extra data (baptism, discipleship, etc.)</summary>// Propiedad mapeada a JSONB
    // Usamos JsonDocument porque la estructura es variable (pueden ser libros, bautizos, etc.)
    public JsonDocument? ExtraData { get; set; }

    // --- NUEVA RELACIÓN CENTRALIZADA ---
    /// <summary>
    /// Colección de todas las participaciones del miembro en distintas estructuras.
    /// (Ej: Líder en Red Jóvenes, Servidor en Ujieres, etc.)
    /// </summary>
    public ICollection<OrganizationMember> OrganizationMemberships { get; set; } = new List<OrganizationMember>();

    // Relación con el Usuario del Sistema (Login)
    // 🔥 LA CONTRAPARTE: Le dice "Este User se conecta con la propiedad 'Member' del otro lado"
    [InverseProperty("Member")]
    public User? User { get; set; }


}
