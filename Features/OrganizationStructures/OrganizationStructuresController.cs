using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaBackend.Features.OrganizationStructures;

[ApiController]
[Route("api/organization-structures")]
[Authorize]
public class OrganizationStructuresController : ControllerBase
{
    private readonly OrganizationStructureService _service;

    public OrganizationStructuresController(OrganizationStructureService service)
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

    // Endpoint extra: Obtener sub-estructuras (Ej: Dame todos los grupos de la Red X)
    [HttpGet("{id:int}/children")]
    public async Task<IActionResult> GetChildren(int id)
    {
        var result = await _service.GetChildrenAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrganizationStructureCreateUpdateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] OrganizationStructureCreateUpdateDto dto)
    {
        try
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
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
            // 🔥 AQUÍ ENVIAMOS EL MENSAJE CLARO AL FRONTEND ("No se puede borrar porque tiene hijos...")
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
    [HttpPost("{id:int}/clone")]
    public async Task<IActionResult> Clone(int id, [FromBody] CloneOrganizationStructureDto dto)
    {
        try
        {
            var success = await _service.CloneDeepAsync(id, dto);
            if (!success) return NotFound(new { message = "Estructura original no encontrada." });
            return NoContent(); // 204: Éxito, sin contenido extra
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}