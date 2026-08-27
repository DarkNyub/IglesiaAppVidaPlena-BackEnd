namespace IglesiaBackend.Features.OrganizationMembers;

public class OrganizationMemberService
{
    private readonly OrganizationMemberRepository _repository;

    public OrganizationMemberService(OrganizationMemberRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OrganizationMemberDto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(OrganizationMemberMapper.ToDto).ToList();
    }

    public async Task<OrganizationMemberDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : OrganizationMemberMapper.ToDto(entity);
    }

    // Métodos específicos para vistas filtradas
    public async Task<List<OrganizationMemberDto>> GetByStructureAsync(int structureId)
    {
        var list = await _repository.GetByStructureAsync(structureId);
        return list.Select(OrganizationMemberMapper.ToDto).ToList();
    }

    public async Task<List<OrganizationMemberDto>> GetByMemberAsync(int memberId)
    {
        var list = await _repository.GetByMemberAsync(memberId);
        return list.Select(OrganizationMemberMapper.ToDto).ToList();
    }

    public async Task<OrganizationMemberDto> CreateAsync(OrganizationMemberCreateUpdateDto dto)
    {
        // TODO: Aquí podrías validar que el MemberId y StructureId existan realmente 
        // llamando a sus repositorios si quieres ser muy estricto, 
        // o dejar que falle la FK en base de datos.

        var entity = OrganizationMemberMapper.ToEntity(dto);
        await _repository.CreateAsync(entity);

        return OrganizationMemberMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, OrganizationMemberCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        OrganizationMemberMapper.UpdateEntity(entity, dto);
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
    public async Task<bool> RestoreAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        entity.IsDeleted = false;

        // Hacemos un Update normal para que se guarde el IsDeleted = false
        entity.IsDeleted = false;
        await _repository.UpdateSimpleAsync(entity);

        return true;
    }
}