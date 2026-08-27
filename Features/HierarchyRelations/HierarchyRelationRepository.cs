using IglesiaBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.HierarchyRelations;

public class HierarchyRelationRepository
{
    private readonly AppDbContext _context;

    public HierarchyRelationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<HierarchyRelation>> GetAllAsync()
    {
        return await _context.HierarchyRelations
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<HierarchyRelation?> GetByIdAsync(int id)
    {
        return await _context.HierarchyRelations
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<HierarchyRelation> CreateAsync(HierarchyRelation entity)
    {
        _context.HierarchyRelations.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(HierarchyRelation entity)
    {
        _context.HierarchyRelations.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(HierarchyRelation entity)
    {
        _context.HierarchyRelations.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
