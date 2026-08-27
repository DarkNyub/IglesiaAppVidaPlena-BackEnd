using System.ComponentModel.DataAnnotations;

namespace IglesiaBackend.Features.CommunityNetworks;

public class CommunityNetworkDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? NetworkCode { get; set; }
    public int? LeaderId { get; set; }
    public string? LeaderName { get; set; }
}
public class CommunityNetworkCreateUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? NetworkCode { get; set; }

    public int? LeaderId { get; set; }
}
