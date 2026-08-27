using IglesiaBackend.Features.RecordTypes.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaBackend.Features.RecordTypes;

[ApiController]
[Route("api/record-types")]
[Authorize]
public class RecordTypeController : ControllerBase
{
    private readonly RecordTypeService _service;

    public RecordTypeController(RecordTypeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RecordTypeCreateUpdateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] RecordTypeCreateUpdateDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("event/{eventId}/structure")]
    public async Task<IActionResult> GetStructure(int eventId)
    {
        var structure = await _service.GetFormStructureForEventAsync(eventId);
        if (structure == null)
            return NotFound("Este evento no tiene un formulario configurado.");

        return Ok(structure);
    }
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        var success = await _service.RestoreAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}