using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaBackend.Features.OrganizationTypes;

[ApiController]
[Route("api/organization-types")]
[Authorize] // Asegura que solo usuarios logueados accedan
public class OrganizationTypesController : ControllerBase
{
    private readonly OrganizationTypeService _service;

    public OrganizationTypesController(OrganizationTypeService service)
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
    public async Task<IActionResult> Create([FromBody] OrganizationTypeCreateUpdateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] OrganizationTypeCreateUpdateDto dto)
    {
        var success = await _service.UpdateAsync(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            // 🔥 CAPTURAR EL ERROR PARA EL FRONTEND
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        var success = await _service.RestoreAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}