using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaBackend.Features.RecordTypeFields;

[ApiController]
[Route("api/record-type-fields")]
[Authorize]
public class RecordTypeFieldController : ControllerBase
{
    private readonly RecordTypeFieldService _service;

    public RecordTypeFieldController(RecordTypeFieldService service)
    {
        _service = service;
    }

    [HttpGet("by-record-type/{recordTypeId:int}")]
    public async Task<IActionResult> GetByRecordType(int recordTypeId)
        => Ok(await _service.GetByRecordTypeAsync(recordTypeId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RecordTypeFieldCreateUpdateDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] RecordTypeFieldCreateUpdateDto dto)
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
}