using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.CommunityNetworks;

public class CommunityNetworkService
{
    private readonly CommunityNetworkRepository _repository;

    public CommunityNetworkService(CommunityNetworkRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CommunityNetworkDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(CommunityNetworkMapper.ToDto).ToList();
    }

    public async Task<CommunityNetworkDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : CommunityNetworkMapper.ToDto(entity);
    }

    public async Task<CommunityNetworkDto> CreateAsync(CommunityNetworkCreateUpdateDto dto)
    {
        var entity = new CommunityNetwork();
        CommunityNetworkMapper.MapCreateUpdateDto(dto, entity);

        await _repository.AddAsync(entity);
        entity = await _repository.GetByIdAsync(entity.Id);
        return CommunityNetworkMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, CommunityNetworkCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        CommunityNetworkMapper.MapCreateUpdateDto(dto, entity);
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
