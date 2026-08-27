using IglesiaBackend.Features;

namespace IglesiaBackend.Features.OrganizationTypes;

public class OrganizationTypeService
{
    private readonly OrganizationTypeRepository _repository;

    public OrganizationTypeService(OrganizationTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OrganizationTypeDto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(OrganizationTypeMapper.ToDto).ToList();
    }

    public async Task<OrganizationTypeDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : OrganizationTypeMapper.ToDto(entity);
    }

    public async Task<OrganizationTypeDto> CreateAsync(OrganizationTypeCreateUpdateDto dto)
    {
        // Regla de negocio: Asegurar Key en mayúsculas
        dto.Key = dto.Key.ToUpper().Trim();

        var entity = OrganizationTypeMapper.ToEntity(dto);
        await _repository.CreateAsync(entity);

        return OrganizationTypeMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, OrganizationTypeCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        // Regla de negocio
        dto.Key = dto.Key.ToUpper().Trim();

        OrganizationTypeMapper.UpdateEntity(entity, dto);
        await _repository.UpdateAsync(entity);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        // 🔥 REGLA DE NEGOCIO: No borrar si está en uso
        bool isInUse = await _repository.IsInUseAsync(id);
        if (isInUse)
        {
            throw new InvalidOperationException("No se puede eliminar este tipo de organización porque existen estructuras activas que lo están usando.");
        }

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