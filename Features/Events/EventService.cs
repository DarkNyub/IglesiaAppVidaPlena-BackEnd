using IglesiaBackend.Features.Events;
using IglesiaBackend.Features.OrganizationStructures;
using System.Security.Claims;

namespace IglesiaBackend.Features.Events;

public class EventService
{
    private readonly EventRepository _repository;
    private readonly OrganizationStructureRepository _orgRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EventService(EventRepository repository, OrganizationStructureRepository orgRepository, IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _orgRepository = orgRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task<List<int>?> GetAllowedStructureIdsAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null) return null;

        var role = user.FindFirst(ClaimTypes.Role)?.Value ?? "";
        // Si es Admin, devuelve null (Ver todo)
        if (role.Contains("Admin", StringComparison.OrdinalIgnoreCase) || role.Contains("Super", StringComparison.OrdinalIgnoreCase)) 
            return null;

        var memberIdClaim = user.FindFirst("memberId")?.Value;
        if (string.IsNullOrEmpty(memberIdClaim) || !int.TryParse(memberIdClaim, out int memberId))
            return new List<int>(); // Si no tiene un miembro atado, ve cero resultados

        return await _orgRepository.GetDescendingStructureIdsAsync(memberId);
    }

    public async Task<List<EventDto>> GetAllAsync()
    {
        var allowedIds = await GetAllowedStructureIdsAsync();
        var events = await _repository.GetAllAsync(allowedIds);
        return events.Select(EventMapper.ToDto).ToList();
    }

    public async Task<EventDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : EventMapper.ToDto(entity);
    }

    public async Task<EventDto> CreateAsync(EventCreateUpdateDto dto)
    {
        Validate(dto);

        var entity = EventMapper.ToEntity(dto);
        await _repository.CreateAsync(entity, dto.RecordTypeIds);

        return EventMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, EventCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        Validate(dto);

        EventMapper.UpdateEntity(entity, dto);
        await _repository.UpdateAsync(entity, dto.RecordTypeIds);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        await _repository.DeleteAsync(entity);
        return true;
    }

    private static void Validate(EventCreateUpdateDto dto)
    {
        // Regla 1: Si es semanal, obligatoriamente debe tener días seleccionados
        if (dto.RecurrenceType == "WEEKLY" && (dto.RecurringDays == null || (int)dto.RecurringDays == 0))
            throw new ArgumentException("Un evento recurrente semanal debe tener al menos un día definido.");

        // Regla 2: Si tiene fecha de fin, no puede ser nula
        if (dto.EndType == "UNTIL_DATE" && dto.EndDate == null)
            throw new ArgumentException("Debe especificar una fecha de finalización válida.");

        // Regla 3: Si tiene límite de repeticiones, no puede ser nulo ni cero
        if (dto.EndType == "AFTER_OCCURRENCES" && (dto.MaxOccurrences == null || dto.MaxOccurrences <= 0))
            throw new ArgumentException("Debe especificar un número máximo de repeticiones válido (mayor a 0).");

        // Regla 4: El intervalo no puede ser menor a 1
        if (dto.RecurrenceType != "NONE" && dto.RecurrenceInterval < 1)
            throw new ArgumentException("El intervalo de repetición debe ser al menos 1.");
    }

    public async Task<bool> RestoreAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        entity.IsDeleted = false;
        await _repository.UpdateSimpleAsync(entity);
        return true;
    }
}