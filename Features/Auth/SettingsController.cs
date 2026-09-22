using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IglesiaBackend.Features.Shared.Services;

namespace IglesiaBackend.Features.Settings;

[ApiController]
[Route("api/settings")]
[Authorize] // Requiere estar logueado como admin para subir los logos
public class SettingsController : ControllerBase
{
    // Asegúrate de inyectar aquí la interfaz de tu servicio de GitHub
    private readonly GitHubStorageService _gitHubStorage; // 🔥 NUEVO

    public SettingsController(GitHubStorageService gitHubStorage)
    {
        _gitHubStorage = gitHubStorage;
    }

    [HttpPost("upload-system-image")]
    public async Task<IActionResult> UploadSystemImage([FromBody] UploadImageRequest dto)
    {
        if (string.IsNullOrEmpty(dto.Base64Image))
            return BadRequest(new { message = "La imagen en Base64 es requerida." });

        try
        {
            // Generamos un nombre único basado en el tipo (login, member_default, org_default) y la fecha
            string fileName = $"{dto.Type}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.jpg";
            
            // Reutilizamos tu lógica de GitHub (guardándolas en una carpeta "system")
            string url = await _gitHubStorage.UploadImageAsync(dto.Base64Image, "system", fileName);
            
            // Devolvemos la URL para que Flutter la guarde en el LocalStorage
            return Ok(new { url });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al subir la imagen: {ex.Message}" });
        }
    }
}

public class UploadImageRequest
{
    public string Base64Image { get; set; } = string.Empty;
    public string Type { get; set; } = "system"; 
}