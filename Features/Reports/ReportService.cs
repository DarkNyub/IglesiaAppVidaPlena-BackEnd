using IglesiaBackend.Features.Reports.Dtos;

namespace IglesiaBackend.Features.Reports;

public class ReportService
{
    private readonly ReportRepository _repository;

    public ReportService(ReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ReportDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(ReportMapper.ToDto).ToList();
    }

    public async Task<ReportDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : ReportMapper.ToDto(entity);
    }

    public async Task<ReportDto> CreateAsync(ReportCreateUpdateDto dto)
    {
        var entity = ReportMapper.ToEntity(dto);
        await _repository.AddAsync(entity);
        return ReportMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, ReportCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        ReportMapper.UpdateEntity(entity, dto);
        await _repository.UpdateAsync(entity);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        await _repository.DeleteAsync(entity);
        return true;
    }
    // 🔥 AGREGAR MÉTODO DE RESTAURAR
    public async Task<bool> RestoreAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        entity.IsDeleted = false;
        await _repository.UpdateSimpleAsync(entity);
        return true;
    }
    public async Task<List<ReportColumnDto>> GetAvailableColumnsAsync(int recordTypeId)
    {
        return await _repository.GetAvailableColumnsAsync(recordTypeId);
    }

    public async Task<List<Dictionary<string, object>>> GenerateFlatReportAsync(DynamicReportRequestDto request)
    {
        return await _repository.GenerateFlatReportAsync(request);
    }
}