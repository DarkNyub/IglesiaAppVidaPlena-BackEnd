using IglesiaBackend.Data;
using IglesiaBackend.Features;
using IglesiaBackend.Features.SystemRoles;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.OrganizationTypes;

public class OrganizationTypeRepository
{
    private readonly AppDbContext _context;

    public OrganizationTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrganizationType>> GetAllAsync()
    {
        return await _context.OrganizationTypes
            .AsNoTracking()
            .IgnoreQueryFilters() // <--- OBLIGATORIO para ver los "rojos"
            .OrderBy(r => r.Id)
            .ToListAsync();
    }

    public async Task<OrganizationType?> GetByIdAsync(int id)
    {
        return await _context.OrganizationTypes
            .IgnoreQueryFilters() // <--- OBLIGATORIO para ver los "rojos"
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<OrganizationType> CreateAsync(OrganizationType entity)
    {
        _context.OrganizationTypes.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(OrganizationType entity)
    {
        _context.OrganizationTypes.Update(entity);
        await _context.SaveChangesAsync();
    }
    // Guardado ligero (útil para Restore/SoftDelete sin validaciones complejas)
    public async Task UpdateSimpleAsync(OrganizationType entity)
    {
        _context.OrganizationTypes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(OrganizationType entity)
    {
        _context.OrganizationTypes.Remove(entity);
        await _context.SaveChangesAsync();
    }
    // Verifica si hay Estructuras activas usando este Tipo
    public async Task<bool> IsInUseAsync(int typeId)
    {
        return await _context.OrganizationStructures
            .AnyAsync(s => s.OrganizationTypeId == typeId && !s.IsDeleted);
    }
}