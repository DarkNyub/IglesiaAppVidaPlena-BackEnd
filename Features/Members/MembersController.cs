using Microsoft.AspNetCore.Authorization;
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
        return Ok(await _service.GetAllAsync());
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
}