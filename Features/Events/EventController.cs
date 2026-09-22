using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaBackend.Features.Events;

[ApiController]
[Route("api/events")]
[Authorize]
public class EventController : ControllerBase
{
    private readonly EventService _service;

    public EventController(EventService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EventCreateUpdateDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EventCreateUpdateDto dto)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        var success = await _service.RestoreAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
    // 🔥 EL ENDPOINT QUE FLUTTER LLAMA PARA ARMAR EL BOTTOM SHEET O EL FORMULARIO DIRECTO
    [HttpGet("{id:int}/structure")]
    public async Task<IActionResult> GetEventStructure(int id)
    {
        // Extraemos quién es el que está consultando
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "";
        var memberIdClaim = User.FindFirst("memberId")?.Value;
        int? memberId = string.IsNullOrEmpty(memberIdClaim) ? null : int.Parse(memberIdClaim);

        var structures = await _service.GetEventFormStructuresAsync(id, userRole, memberId);
        
        if (structures == null) 
            return NotFound(new { message = "No tienes formularios asignados para este evento según tus roles actuales." });

        // Retorna la LISTA de formularios permitidos
        return Ok(structures);
    }
}