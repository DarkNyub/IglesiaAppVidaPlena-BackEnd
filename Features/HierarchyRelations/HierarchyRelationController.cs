using IglesiaBackend.Features.HierarchyRelations.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaBackend.Features.HierarchyRelations;

[ApiController]
[Route("api/hierarchy-relations")]
[Authorize]
public class HierarchyRelationController : ControllerBase
{
    private readonly HierarchyRelationService _service;

    public HierarchyRelationController(HierarchyRelationService service)
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
    public async Task<IActionResult> Create(HierarchyRelationCreateUpdateDto dto)
    {
        // luego sale del JWT 😉

        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, HierarchyRelationCreateUpdateDto dto)
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
}
