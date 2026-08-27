using IglesiaBackend.Features.CommunityNetworks;
using IglesiaBackend.Features.Members;
using IglesiaBackend.Shared.Entities;

public class LifeGroup : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Ej: Grupo Betania

    // Quién es el Líder del Grupo
    public int? LeaderId { get; set; }
    public Member? Leader { get; set; }

    // A qué Red pertenece
    public int CommunityNetworkId { get; set; }
    public CommunityNetwork CommunityNetwork { get; set; } = null!;

    // Miembros que asisten a este grupo (Ovejas)
    // Nota: Necesitaremos una tabla intermedia o una FK en Member si un miembro solo va a 1 grupo.
    // Por flexibilidad, sugiero MemberLifeGroup o FK en Member. 
    // Por ahora asumamos FK en Member: public int? LifeGroupId { get; set; }
}