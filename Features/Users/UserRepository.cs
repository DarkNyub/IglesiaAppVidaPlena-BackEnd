using IglesiaBackend.Data;
using IglesiaBackend.Features.UserSystemRoles; // Importante
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.Users;

public class UserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .IgnoreQueryFilters() // Para ver usuarios borrados (Soft Deleted) en el admin
            .Include(u => u.Member)
            .Include(u => u.UserSystemRoles)
                .ThenInclude(usr => usr.SystemRole)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .IgnoreQueryFilters() // Necesario para editar usuarios inactivos/borrados
            .Include(u => u.Member)
            .Include(u => u.UserSystemRoles)
                .ThenInclude(usr => usr.SystemRole)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> AddAsync(User entity)
    {
        _context.Users.Add(entity);
        await _context.SaveChangesAsync();
        // Recarga para tener el Member y devolver DTO completo
        await _context.Entry(entity).Reference(u => u.Member).LoadAsync();
        return entity;
    }

    // 🔥 SMART SYNC PARA ROLES DE SISTEMA
    public async Task UpdateAsync(User entity, List<int> newRoleIds)
    {
        // 1. Actualizamos datos básicos del User
        _context.Users.Update(entity);

        // 2. Sincronizar Roles (UserSystemRoles)
        // Traemos TODOS (incluso los borrados) para poder reactivarlos
        var allDbRoles = await _context.UserSystemRoles
            .IgnoreQueryFilters() // <--- CLAVE: Ver los borrados
            .Where(x => x.UserId == entity.Id)
            .ToListAsync();

        // A) PROCESAR LA LISTA NUEVA (Insertar o Reactivar)
        foreach (var roleId in newRoleIds)
        {
            var existingRole = allDbRoles.FirstOrDefault(x => x.SystemRoleId == roleId);

            if (existingRole != null)
            {
                // Si existe, aseguramos que esté vivo
                if (existingRole.IsDeleted) // Asumiendo que tiene IsDeleted
                {
                    existingRole.IsDeleted = false;
                    // existingRole.LastModifiedDate = DateTime.UtcNow; // Opcional
                    _context.UserSystemRoles.Update(existingRole);
                }
            }
            else
            {
                // Si no existe, lo creamos
                _context.UserSystemRoles.Add(new UserSystemRole
                {
                    UserId = entity.Id,
                    SystemRoleId = roleId,
                    IsDeleted = false
                });
            }
        }

        // B) ELIMINAR LOS QUE SOBRAN (Soft Delete)
        // Los que están en BD pero NO vinieron en la nueva lista
        var rolesToDelete = allDbRoles
            .Where(dbRole => !newRoleIds.Contains(dbRole.SystemRoleId))
            .ToList();

        foreach (var role in rolesToDelete)
        {
            if (!role.IsDeleted) // Solo si no estaba ya borrado
            {
                // 🔥 USAR REMOVE EN LUGAR DE UPDATE
                _context.UserSystemRoles.Remove(role);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User entity)
    {
        // 🔥 LA MAGIA: Dejamos que el DbContext haga el Soft Delete y la Auditoría
        _context.Users.Remove(entity);
        await _context.SaveChangesAsync();
    }

    // Método login (solo activos y no borrados)
    public async Task<User?> GetForLoginByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.Member)
            .Include(u => u.UserSystemRoles)
                .ThenInclude(usr => usr.SystemRole)
            .Where(u => !u.IsDeleted) // 🔥 SOLO filtramos los eliminados. Dejamos pasar los inactivos.
            .FirstOrDefaultAsync(u => u.Username == username);
    }
    // 🔥 NUEVO: Guardado ligero para cambiar estados sin tocar roles
    public async Task UpdateSimpleAsync(User entity)
    {
        _context.Users.Update(entity);
        // Al no tocar la colección UserSystemRoles, EF Core la ignora
        await _context.SaveChangesAsync();
    }
}