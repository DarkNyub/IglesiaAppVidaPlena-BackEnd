using IglesiaBackend.Features.Reports.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace IglesiaBackend.Features.Reports;

[ApiController]
[Route("api/reports")]
[Authorize] // Asegura que solo usuarios logueados accedan
public class ReportController : ControllerBase
{
    private readonly ReportService _service;

    public ReportController(ReportService service)
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
    public async Task<IActionResult> Create([FromBody] ReportCreateUpdateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ReportCreateUpdateDto dto)
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
    // 🔥 AGREGAR ENDPOINT DE RESTAURAR
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        var success = await _service.RestoreAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    // 1. Flutter llama aquí para pintar los checkboxes
    [HttpGet("available-columns/{recordTypeId:int}")]
    public async Task<IActionResult> GetAvailableColumns(int recordTypeId)
    {
        var columns = await _service.GetAvailableColumnsAsync(recordTypeId);
        return Ok(columns);
    }

    // 2. Flutter manda los checkboxes elegidos y recibe la tabla de datos
    [HttpPost("generate-flat")]
    public async Task<IActionResult> GenerateFlatReport([FromBody] DynamicReportRequestDto request)
    {
        if (request.SelectedColumns == null || !request.SelectedColumns.Any())
            return BadRequest(new { message = "Debe seleccionar al menos una columna para el reporte." });

        try
        {
            var data = await _service.GenerateFlatReportAsync(request);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}