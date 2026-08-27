using IglesiaBackend.Features.UserSystemRoles.DTOs;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace IglesiaBackend.Features.UserSystemRoles;

public class UserSystemRoleService
{
    private readonly UserSystemRoleRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserSystemRoleService(
        UserSystemRoleRepository repository,
        IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?
            .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return userIdClaim != null ? int.Parse(userIdClaim) : 0;
    }

    public async Task<List<UserSystemRolDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(UserSystemRoleMapper.ToDto).ToList();
    }

    public async Task<UserSystemRolDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : UserSystemRoleMapper.ToDto(entity);
    }

    public async Task<UserSystemRolDto> CreateAsync(UserSystemRolCreateUpdateDto dto)
    {
        // Forzamos validación de Auth
        _ = GetCurrentUserId();

        var entity = new UserSystemRole
        {
            UserId = dto.UserId,
            SystemRoleId = dto.SystemRoleId
        };

        await _repository.CreateAsync(entity);
        return UserSystemRoleMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, UserSystemRolCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        entity.UserId = dto.UserId;
        entity.SystemRoleId = dto.SystemRoleId;

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
    // 🔥 AGREGAR MÉTODO DE RESTAURAR
    public async Task<bool> RestoreAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        entity.IsDeleted = false;
        await _repository.UpdateAsync(entity);
        return true;
    }
}