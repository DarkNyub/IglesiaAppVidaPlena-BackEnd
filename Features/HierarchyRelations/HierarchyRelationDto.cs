namespace IglesiaBackend.Features.HierarchyRelations.DTOs;

public class HierarchyRelationDto
{
    public int Id { get; set; }

    /// <summary>
    /// Miembro que ejerce supervisión
    /// </summary>
    public int SupervisorId { get; set; }

    /// <summary>
    /// Miembro que está bajo supervisión
    /// </summary>
    public int SubordinateId { get; set; }

    /// <summary>
    /// Red comunitaria asociada (opcional)
    /// </summary>
    public int? CommunityNetworkId { get; set; }

    /// <summary>
    /// Ministerio asociado (opcional)
    /// </summary>
    public int? MinistryId { get; set; }
}

public class HierarchyRelationCreateUpdateDto
{
    public int SupervisorId { get; set; }
    public int SubordinateId { get; set; }
    public int? CommunityNetworkId { get; set; }
    public int? MinistryId { get; set; }
}