namespace IglesiaBackend.Features.Ministries;

public class MinistryService
{
    private readonly MinistryRepository _repository;

    public MinistryService(MinistryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MinistryDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MinistryMapper.ToDto).ToList();
    }

    public async Task<MinistryDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : MinistryMapper.ToDto(entity);
    }

    public async Task<MinistryDto> CreateAsync(MinistryCreateUpdateDto dto)
    {
        var entity = new Ministry();
        MinistryMapper.MapCreateUpdateDto(dto, entity);

        await _repository.AddAsync(entity);
        return MinistryMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, MinistryCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        MinistryMapper.MapCreateUpdateDto(dto, entity);

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
}
