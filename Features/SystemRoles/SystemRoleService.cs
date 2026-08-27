using IglesiaBackend.Features.SystemRoles.Dtos;
using System.Security.Claims;

namespace IglesiaBackend.Features.SystemRoles;

public class SystemRoleService
{
    private readonly SystemRoleRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SystemRoleService(
        SystemRoleRepository repository,
        IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetUserIdFromJwt()
    {
        var userId = _httpContextAccessor.HttpContext?
            .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Invalid JWT token");

        return int.Parse(userId);
    }

    public async Task<List<SystemRoleDto>> GetAllAsync()
    {
        var roles = await _repository.GetAllAsync();
        return roles.Select(SystemRoleMapper.ToDto).ToList();
    }

    public async Task<SystemRoleDto?> GetByIdAsync(int id)
    {
        var role = await _repository.GetByIdAsync(id);
        return role == null ? null : SystemRoleMapper.ToDto(role);
    }

    public async Task<SystemRoleDto> CreateAsync(CreateUpdateSystemRoleDto dto)
    {
        // Validamos Auth (aunque el Controller ya tiene [Authorize])
        _ = GetUserIdFromJwt();

        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            var existing = await _repository.GetByNameAsync(dto.Name);
            if (existing != null)
                throw new InvalidOperationException($"El rol de sistema '{dto.Name}' ya existe.");
        }

        var entity = SystemRoleMapper.ToEntity(dto);
        await _repository.AddAsync(entity);

        return SystemRoleMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, CreateUpdateSystemRoleDto dto)
    {
        _ = GetUserIdFromJwt();

        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        SystemRoleMapper.UpdateEntity(entity, dto);
        await _repository.UpdateAsync(entity);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _ = GetUserIdFromJwt();

        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

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