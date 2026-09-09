using IglesiaBackend.Features.RegistryEvents.Dtos;
using System.Security.Claims;

namespace IglesiaBackend.Features.RegistryEvents;

/// <summary>
/// Business logic for RegistryEvent
/// </summary>
public class RegistryEventService
{
    private readonly RegistryEventRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RegistryEventService(
        RegistryEventRepository repository,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetUserIdFromJwt()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?
            .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("Invalid JWT token");

        // NOTA: Asumimos que el ID del token corresponde al MemberId.
        // Si tu sistema de Auth usa una tabla User separada con ID distinto al Member,
        // aquí deberías buscar el MemberId asociado al User.
        return int.Parse(userIdClaim);
    }
    private int GetLeaderIdFromJwt()
    {
        // 🔥 LEEMOS EL CLAIM ESPECÍFICO QUE CREASTE EN JwtService
        var memberIdClaim = _httpContextAccessor.HttpContext?
            .User.FindFirst("memberId")?.Value;

        if (string.IsNullOrEmpty(memberIdClaim))
            throw new UnauthorizedAccessException("El usuario no tiene un perfil de miembro asociado.");

        return int.Parse(memberIdClaim);
    }

    // Extraemos la información de forma segura por si un SuperAdmin no tiene perfil de miembro
    public async Task<List<RegistryEventDto>> GetAllAsync()
    {
        var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        var memberIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("memberId")?.Value;
        int? currentMemberId = string.IsNullOrEmpty(memberIdClaim) ? null : int.Parse(memberIdClaim);

        var entities = await _repository.GetAllAsync(userRole, currentMemberId);
        return entities.Select(RegistryEventMapper.ToDto).ToList();
    }

    public async Task<List<RegistryEventDto>> GetByEventAsync(int eventId)
    {
        var entities = await _repository.GetByEventAsync(eventId);
        return entities.Select(RegistryEventMapper.ToDto).ToList();
    }

    public async Task<RegistryEventDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : RegistryEventMapper.ToDto(entity);
    }

    public async Task<RegistryEventDto> CreateAsync(RegistryEventCreateUpdateDto dto)
    {
        var leaderId = GetLeaderIdFromJwt();

        var entity = RegistryEventMapper.ToEntity(dto, leaderId);
        await _repository.AddAsync(entity);

        return RegistryEventMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, RegistryEventCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        RegistryEventMapper.UpdateEntity(entity, dto);
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
        await _repository.UpdateSimpleAsync(entity);
        return true;
    }
}