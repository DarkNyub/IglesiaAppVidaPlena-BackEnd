using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaBackend.Features.Settings;

[ApiController]
[Route("api/settings")]
public class SettingsController : ControllerBase
{
    private readonly SettingsService _service;

    public SettingsController(SettingsService service)
    {
        _service = service;
    }

    // Endpoint abierto para que el Login descargue las imágenes sin estar logueado
    [HttpGet("public")]
    [AllowAnonymous] 
    public async Task<IActionResult> GetPublicSettings()
    {
        var settings = await _service.GetAllSettingsAsync();
        return Ok(settings);
    }

    // Endpoint protegido para que solo los admins puedan subir logos
    [HttpPost("upload-system-image")]
    [Authorize(Roles = "SuperAdmin,Admin")] 
    public async Task<IActionResult> UploadSystemImage([FromBody] UploadImageRequest dto)
    {
        try
        {
            string url = await _service.UploadAndSaveImageAsync(dto.Base64Image, dto.Type);
            return Ok(new { url });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al subir la imagen: {ex.Message}" });
        }
    }
}
