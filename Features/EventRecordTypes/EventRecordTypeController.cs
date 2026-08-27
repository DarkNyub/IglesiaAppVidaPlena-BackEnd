using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaBackend.Features.EventRecordTypes;

[ApiController]
[Route("api/event-record-types")]
[Authorize]
public class EventRecordTypeController : ControllerBase
{
    private readonly EventRecordTypeService _service;

    public EventRecordTypeController(EventRecordTypeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // GET api/event-record-types/by-event/5
    [HttpGet("by-event/{eventId:int}")]
    public async Task<IActionResult> GetByEvent(int eventId)
    {
        return Ok(await _service.GetByEventAsync(eventId));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EventRecordTypeCreateUpdateDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
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
}