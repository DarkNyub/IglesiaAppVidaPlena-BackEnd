using IglesiaBackend.Features.RecordTypeFields;
using IglesiaBackend.Features.RecordTypes.Dtos;

namespace IglesiaBackend.Features.RecordTypes;

public class RecordTypeService
{
    private readonly RecordTypeRepository _repository;

    public RecordTypeService(RecordTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<RecordTypeDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(RecordTypeMapper.ToDto).ToList();
    }

    public async Task<RecordTypeDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : RecordTypeMapper.ToDto(entity);
    }

    public async Task<RecordTypeDto> CreateAsync(RecordTypeCreateUpdateDto dto)
    {
        var entity = RecordTypeMapper.ToEntity(dto);
        await _repository.AddAsync(entity);

        // Recargar para traer nombres de Targeting si hace falta (opcional)
        var created = await _repository.GetByIdAsync(entity.Id);
        return RecordTypeMapper.ToDto(created!);
    }

    public async Task<bool> UpdateAsync(int id, RecordTypeCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        // 1. Actualizar datos base (Nombre, Desc, Targeting)
        RecordTypeMapper.UpdateEntity(entity, dto);

        // 2. Delegar la sincronización de campos al repositorio
        await _repository.UpdateWithFieldsAsync(entity, dto.Fields);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        await _repository.DeleteAsync(entity);
        return true;
    }

    public async Task<FormStructureDto?> GetFormStructureForEventAsync(int eventId)
    {
        var recordType = await _repository.GetByEventIdAsync(eventId);
        if (recordType == null) return null;

        return new FormStructureDto
        {
            RecordTypeId = recordType.Id,
            FormName = recordType.Name,
            // Reutilizamos el mapper de campos
            Fields = recordType.Fields.Select(RecordTypeFieldMapper.ToDto).ToList()
        };
    }

    // Y el Restore genérico que ya conoces
    public async Task<bool> RestoreAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        // Opcional: Podrías validar que el Padre de este nodo esté activo antes de revivirlo,
        // pero por ahora, revivirlo a secas está bien.
        entity.IsDeleted = false;
        await _repository.UpdateSimpleAsync(entity);
        return true;
    }
}