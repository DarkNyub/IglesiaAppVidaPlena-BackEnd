namespace IglesiaBackend.Features.EventRecordTypes;

public class EventRecordTypeService
{
    private readonly EventRecordTypeRepository _repository;

    public EventRecordTypeService(EventRecordTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<EventRecordTypeDto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(EventRecordTypeMapper.ToDto).ToList();
    }

    // Método extra para filtrar desde el controller si se necesita
    public async Task<List<EventRecordTypeDto>> GetByEventAsync(int eventId)
    {
        var list = await _repository.GetByEventAsync(eventId);
        return list.Select(EventRecordTypeMapper.ToDto).ToList();
    }

    public async Task<EventRecordTypeDto> CreateAsync(EventRecordTypeCreateUpdateDto dto)
    {
        if (await _repository.ExistsAsync(dto.EventId, dto.RecordTypeId))
            throw new InvalidOperationException("Este tipo de registro ya está asociado al evento.");

        var entity = new EventRecordType();
        EventRecordTypeMapper.UpdateEntity(entity, dto);

        await _repository.AddAsync(entity);
        return EventRecordTypeMapper.ToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        await _repository.DeleteAsync(entity);
        return true;
    }
}