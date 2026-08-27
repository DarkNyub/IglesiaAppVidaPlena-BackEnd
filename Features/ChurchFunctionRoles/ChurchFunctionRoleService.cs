namespace IglesiaBackend.Features.ChurchFunctionRoles;

public class ChurchFunctionRoleService
{
    private readonly ChurchFunctionRoleRepository _repository;

    public ChurchFunctionRoleService(ChurchFunctionRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ChurchFunctionRoleDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(ChurchFunctionRoleMapper.ToDto).ToList();
    }

    public async Task<ChurchFunctionRoleDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : ChurchFunctionRoleMapper.ToDto(entity);
    }

    public async Task<ChurchFunctionRoleDto> CreateAsync(
        ChurchFunctionRoleCreateUpdateDto dto)
    {
        var entity = new ChurchFunctionRole();

        ChurchFunctionRoleMapper.MapCreateUpdateDto(dto, entity);

        await _repository.AddAsync(entity);
        return ChurchFunctionRoleMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(
        int id,
        ChurchFunctionRoleCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        ChurchFunctionRoleMapper.MapCreateUpdateDto(dto, entity);

        await _repository.UpdateAsync(entity);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        // TODO: (Opcional) Validar si este rol está siendo usado en OrganizationMembers antes de borrar

        await _repository.DeleteAsync(entity);
        return true;
    }
    // 🔥 EL MÉTODO DE RESTAURAR QUE FALTABA
    public async Task<bool> RestoreAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        entity.IsDeleted = false; // Revivir
        await _repository.UpdateSimpleAsync(entity);
        return true;
    }
}