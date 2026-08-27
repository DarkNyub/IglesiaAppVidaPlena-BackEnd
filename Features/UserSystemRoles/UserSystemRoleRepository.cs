using IglesiaBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.UserSystemRoles;

public class UserSystemRoleRepository
{
    private readonly AppDbContext _context;

    public UserSystemRoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserSystemRole>> GetAllAsync()
    {
        return await _context.UserSystemRoles
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.User)
            .Include(x => x.SystemRole)
            .ToListAsync();
    }

    // Método útil extra: Ver roles de un usuario específico
    public async Task<List<UserSystemRole>> GetByUserIdAsync(int userId)
    {
        return await _context.UserSystemRoles
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(x => x.UserId == userId)
            .Include(x => x.SystemRole)
            .ToListAsync();
    }

    public async Task<UserSystemRole?> GetByIdAsync(int id)
    {
        return await _context.UserSystemRoles
            .Include(x => x.User)
            .Include(x => x.SystemRole)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<UserSystemRole> CreateAsync(UserSystemRole entity)
    {
        _context.UserSystemRoles.Add(entity);
        await _context.SaveChangesAsync();

        // Recargar relaciones para devolver DTO completo
        await _context.Entry(entity).Reference(x => x.User).LoadAsync();
        await _context.Entry(entity).Reference(x => x.SystemRole).LoadAsync();

        return entity;
    }

    public async Task UpdateAsync(UserSystemRole entity)
    {
        _context.UserSystemRoles.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(UserSystemRole entity)
    {
        _context.UserSystemRoles.Remove(entity);
        await _context.SaveChangesAsync();
    }

}