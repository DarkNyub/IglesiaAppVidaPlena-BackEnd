using IglesiaBackend.Features.HierarchyRelations.DTOs;

namespace IglesiaBackend.Features.HierarchyRelations;

public class HierarchyRelationService
{
    private readonly HierarchyRelationRepository _repository;

    public HierarchyRelationService(HierarchyRelationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<HierarchyRelationDto>> GetAllAsync()
    {
        var relations = await _repository.GetAllAsync();
        return relations.Select(HierarchyRelationMapper.ToDto).ToList();
    }

    public async Task<HierarchyRelationDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : HierarchyRelationMapper.ToDto(entity);
    }

    public async Task<HierarchyRelationDto> CreateAsync(HierarchyRelationCreateUpdateDto dto)
    {
        var entity = HierarchyRelationMapper.ToEntity(dto);

        await _repository.CreateAsync(entity);
        return HierarchyRelationMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, HierarchyRelationCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        HierarchyRelationMapper.UpdateEntity(entity, dto);

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
