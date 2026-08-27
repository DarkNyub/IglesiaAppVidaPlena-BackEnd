using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaBackend.Features.OrganizationMembers;

[ApiController]
[Route("api/organization-members")]
[Authorize]
public class OrganizationMembersController : ControllerBase
{
    private readonly OrganizationMemberService _service;

    public OrganizationMembersController(OrganizationMemberService service)
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

    // GET api/organization-members/by-structure/5
    // Para ver: "¿Quiénes están en la Red de Jóvenes?"
    [HttpGet("by-structure/{structureId:int}")]
    public async Task<IActionResult> GetByStructure(int structureId)
    {
        var result = await _service.GetByStructureAsync(structureId);
        return Ok(result);
    }

    // GET api/organization-members/by-member/10
    // Para ver: "¿Qué cargos tiene Juan?"
    [HttpGet("by-member/{memberId:int}")]
    public async Task<IActionResult> GetByMember(int memberId)
    {
        var result = await _service.GetByMemberAsync(memberId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrganizationMemberCreateUpdateDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            // Capturar error de FK si mandan IDs que no existen
            return BadRequest(new { message = "Error al asignar miembro. Verifique que el miembro, estructura y rol existan.", detail = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] OrganizationMemberCreateUpdateDto dto)
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
    // 🔥 AGREGAR EL ENDPOINT
    [HttpPost("{id:int}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        var success = await _service.RestoreAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}