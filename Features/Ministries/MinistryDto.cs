using System.ComponentModel.DataAnnotations;

namespace IglesiaBackend.Features.Ministries;

public class MinistryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? LeaderId { get; set; }
    public string? LeaderName { get; set; }

}
public class MinistryCreateUpdateDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? Description { get; set; }

    public int? LeaderId { get; set; }
}
