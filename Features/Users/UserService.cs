using IglesiaBackend.Features.Auth;
using IglesiaBackend.Features.Users.Dtos;
using IglesiaBackend.Features.UserSystemRoles;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IglesiaBackend.Features.Users;

public class UserService
{
    private readonly UserRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtService _jwtService;
    private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    public UserService(
        UserRepository repository,
        IHttpContextAccessor httpContextAccessor,
        JwtService jwtService)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _jwtService = jwtService;
    }

    private int GetUserIdFromJwt()
    {
        // ... (Tu lógica existente para obtener ID del token) ...
        var claim = _httpContextAccessor.HttpContext?
            .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim)) return 0; // O lanzar excepción
        return int.Parse(claim);
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();
        return users.Select(UserMapper.ToDto).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user == null ? null : UserMapper.ToDto(user);
    }

    public async Task<UserDto> CreateAsync(UserCreateUpdateDto dto)
    {
        _ = GetUserIdFromJwt();

        var existing = await _repository.GetByUsernameAsync(dto.Username);
        if (existing != null)
            throw new InvalidOperationException("El nombre de usuario ya existe.");

        var entity = UserMapper.ToEntity(dto);
        if (!string.IsNullOrEmpty(dto.Password))
        {
            entity.PasswordHash = _passwordHasher.HashPassword(entity, dto.Password);
        }

        // Agregar roles iniciales
        if (dto.SystemRoleIds != null)
        {
            foreach (var roleId in dto.SystemRoleIds)
            {
                entity.UserSystemRoles.Add(new UserSystemRole
                {
                    SystemRoleId = roleId
                });
            }
        }

        await _repository.AddAsync(entity);
        return UserMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, UserCreateUpdateDto dto)
    {
        _ = GetUserIdFromJwt();

        // 1. Obtener entidad existente
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        // 2. Mapear campos simples (Username, MemberId, IsActive)
        UserMapper.UpdateEntity(entity, dto);

        // 3. Hash de contraseña (solo si se envió una nueva)
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            entity.PasswordHash = _passwordHasher.HashPassword(entity, dto.Password);
        }

        // 4. Guardar y Sincronizar Roles (Enviamos la lista de IDs al Repo)
        await _repository.UpdateAsync(entity, dto.SystemRoleIds);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _ = GetUserIdFromJwt();
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        // Soft delete
        await _repository.DeleteAsync(entity);
        return true;
    }

    public async Task<LoginResultDto?> LoginAsync(LoginDto dto)
    {
        var user = await _repository.GetForLoginByUsernameAsync(dto.Username);

        if (user == null) throw new Exception("Credenciales inválidas.");

        var result = _passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, dto.Password);

        if (result == PasswordVerificationResult.Failed) throw new Exception("Credenciales inválidas.");

        if (!user.IsActive) throw new Exception("Usuario suspendido. Comuníquese con el administrador.");

        var systemRole = user.UserSystemRoles
            .Select(x => x.SystemRole)
            .FirstOrDefault();

        var jwtResult = _jwtService.GenerateToken(user, systemRole);

        return new LoginResultDto
        {
            Token = jwtResult.Token,
            ExpiresAt = jwtResult.ExpiresAt,
            UserId = user.Id,
            MemberId = user.MemberId,
            Username = user.Username,
            SystemRole = systemRole?.Name ?? string.Empty
        };
    }
    // 🔥 NUEVO: Toggle exclusivo para IsActive
    public async Task<bool> ToggleActiveAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        // Solo invertimos el interruptor
        entity.IsActive = !entity.IsActive;

        // Usamos el guardado ligero
        await _repository.UpdateSimpleAsync(entity);

        return true;
    }
    public async Task<bool> RestoreAsync(int id)
    {
        // GetByIdAsync ya usa IgnoreQueryFilters, así que encontrará al borrado
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        // Revivimos
        entity.IsDeleted = false;

        // Guardamos ligero (sin tocar roles)
        await _repository.UpdateSimpleAsync(entity);

        return true;
    }
}