namespace IglesiaBackend.Features.MemberChurchFunctionRoles;

public class MemberChurchFunctionRoleService
{
    private readonly MemberChurchFunctionRoleRepository _repository;

    public MemberChurchFunctionRoleService(
        MemberChurchFunctionRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MemberChurchFunctionRoleDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MemberChurchFunctionRoleMapper.ToDto).ToList();
    }

    public async Task<MemberChurchFunctionRoleDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null
            ? null
            : MemberChurchFunctionRoleMapper.ToDto(entity);
    }

    public async Task<MemberChurchFunctionRoleDto> CreateAsync(
        MemberChurchFunctionRoleCreateUpdateDto dto)
    {
        var entity = new MemberChurchFunctionRole();

        MemberChurchFunctionRoleMapper.MapCreateUpdateDto(dto, entity);
        await _repository.AddAsync(entity);

        return MemberChurchFunctionRoleMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(
        int id,
        MemberChurchFunctionRoleCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        MemberChurchFunctionRoleMapper.MapCreateUpdateDto(dto, entity);

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
