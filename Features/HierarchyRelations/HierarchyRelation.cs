using IglesiaBackend.Features.CommunityNetworks;
using IglesiaBackend.Features.Members;
using IglesiaBackend.Features.Ministries;
using IglesiaBackend.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.HierarchyRelations;

public class HierarchyRelation : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int SupervisorId { get; set; }
    [ForeignKey(nameof(SupervisorId))]
    public Member Supervisor { get; set; }

    [Required]
    public int SubordinateId { get; set; }
    [ForeignKey(nameof(SubordinateId))]
    public Member Subordinate { get; set; }

    public int? CommunityNetworkId { get; set; }
    [ForeignKey(nameof(CommunityNetworkId))]
    public CommunityNetwork? CommunityNetwork { get; set; }

    public int? MinistryId { get; set; }
    [ForeignKey(nameof(MinistryId))]
    public Ministry? Ministry { get; set; }

    public int? LifeGroupId { get; set; }
    [ForeignKey(nameof(LifeGroupId))]
    public LifeGroup? LifeGroup { get; set; }

}
