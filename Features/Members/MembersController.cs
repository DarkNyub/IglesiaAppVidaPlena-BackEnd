using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaBackend.Features.Members;

[ApiController]
[Route("api/members")]
[Authorize]
public class MemberController : ControllerBase
{
    private readonly MemberService _service;

    public MemberController(MemberService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // 🔥 1. INTERCEPTAMOS LA IDENTIDAD DESDE EL TOKEN JWT
        var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";
        var memberIdClaim = User.Claims.FirstOrDefault(c => c.Type == "MemberId")?.Value;
        int? currentMemberId = string.IsNullOrEmpty(memberIdClaim) ? null : int.Parse(memberIdClaim);

        // 2. Pasamos la identidad al servicio
        var members = await _service.GetAllAsync(userRole, currentMemberId);
        return Ok(members);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MemberCreateUpdateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] MemberCreateUpdateDto dto)
    {
        var success = await _service.UpdateAsync(id, dto);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }
    [HttpPost("{id:int}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        // Obtenemos el ID del admin que hace la petición (desde el token)
        // Si no tienes configurado esto, puedes pasar 0 o arreglar tu UserSession
        // var adminId = int.Parse(User.FindFirst("uid")?.Value ?? "0"); 

        var success = await _service.ToggleStatusAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }
    // ==========================================
    // ENDPOINT PARA CARGA MASIVA (Excel)
    // ==========================================
    [HttpPost("{id:int}/bulk-upload")]
    public async Task<IActionResult> BulkUpload(int id, [FromBody] MemberBulkDto bulkDto)
    {
        try
        {
            // Validamos que el JSON no venga nulo o vacío
            if (bulkDto == null || !bulkDto.Members.Any())
                return BadRequest(new { message = "El archivo no contiene miembros válidos." });

            // Enviamos el trabajo pesado al servicio
            var count = await _service.BulkUpsertAsync(bulkDto);
            
            // Devolvemos el conteo para que Flutter lo muestre en el SnackBar
            return Ok(new { count = count, message = "Proceso masivo finalizado" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error procesando archivo: {ex.Message}" });
        }
    }
}