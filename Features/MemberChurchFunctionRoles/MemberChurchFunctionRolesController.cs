using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaBackend.Features.MemberChurchFunctionRoles;

[ApiController]
[Route("api/member-church-function-roles")]
[Authorize]
public class MemberChurchFunctionRolesController : ControllerBase
{
    private readonly MemberChurchFunctionRoleService _service;

    public MemberChurchFunctionRolesController(
        MemberChurchFunctionRoleService service)
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
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] MemberChurchFunctionRoleCreateUpdateDto dto)
    {
        // Luego esto lo sacamos del token (UserContextMiddleware)

        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] MemberChurchFunctionRoleCreateUpdateDto dto)
    {

        var updated = await _service.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
