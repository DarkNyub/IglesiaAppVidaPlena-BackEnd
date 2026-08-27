using IglesiaBackend.Data;
using IglesiaBackend.Features.ChurchFunctionRoles;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.SystemRoles;

public class SystemRoleRepository
{
    private readonly AppDbContext _context;

    public SystemRoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SystemRole>> GetAllAsync()
    {
        return await _context.SystemRoles
            .AsNoTracking()
            .IgnoreQueryFilters() // <--- OBLIGATORIO para ver los "rojos"
            .OrderBy(r => r.Id)
            .ToListAsync();
    }

    public async Task<SystemRole?> GetByIdAsync(int id)
    {
        return await _context.SystemRoles
            .IgnoreQueryFilters() // <--- OBLIGATORIO para ver los "rojos"
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<SystemRole?> GetByNameAsync(string name)
    {
        return await _context.SystemRoles
            .FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<SystemRole> AddAsync(SystemRole entity)
    {
        _context.SystemRoles.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(SystemRole entity)
    {
        _context.SystemRoles.Update(entity);
        await _context.SaveChangesAsync();
    }
    // Guardado ligero (útil para Restore/SoftDelete sin validaciones complejas)
    public async Task UpdateSimpleAsync(SystemRole entity)
    {
        _context.SystemRoles.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(SystemRole entity)
    {
        _context.SystemRoles.Remove(entity);
        await _context.SaveChangesAsync();
    }
}