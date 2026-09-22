using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaBackend.Features.Settings;

[Table("SystemSettings")]
public class SystemSetting
{
    [Key]
    [MaxLength(50)]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Value { get; set; } = string.Empty;

    public string? Description { get; set; }
}
public class UploadImageRequest
{
    public string Base64Image { get; set; } = string.Empty;
    public string Type { get; set; } = "system"; 
}