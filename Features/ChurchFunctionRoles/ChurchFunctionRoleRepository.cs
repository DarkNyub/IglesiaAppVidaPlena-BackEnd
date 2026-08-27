using IglesiaBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.ChurchFunctionRoles;

public class ChurchFunctionRoleRepository
{
    private readonly AppDbContext _context;

    public ChurchFunctionRoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChurchFunctionRole>> GetAllAsync()
    {
        // Ordenamos por autoridad por defecto (útil para dropdowns)
        return await _context.ChurchFunctionRoles
            .AsNoTracking()
            .IgnoreQueryFilters() // 🔥 IMPORTANTE: Traer también los borrados para mostrarlos en rojo
            .OrderByDescending(x => x.AuthorityLevel) // Ordenamos por jerarquía por defecto
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<ChurchFunctionRole?> GetByIdAsync(int id)
    {
        return await _context.ChurchFunctionRoles
            .IgnoreQueryFilters() // Para ver usuarios borrados (Soft Deleted) en el admin
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(ChurchFunctionRole entity)
    {
        _context.ChurchFunctionRoles.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ChurchFunctionRole entity)
    {
        _context.ChurchFunctionRoles.Update(entity);
        await _context.SaveChangesAsync();
    }
    // Guardado ligero (útil para Restore/SoftDelete sin validaciones complejas)
    public async Task UpdateSimpleAsync(ChurchFunctionRole entity)
    {
        _context.ChurchFunctionRoles.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ChurchFunctionRole entity)
    {
        _context.ChurchFunctionRoles.Remove(entity);
        await _context.SaveChangesAsync();
    }
}