using IglesiaBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.Ministries;

public class MinistryRepository
{
    private readonly AppDbContext _context;

    public MinistryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ministry>> GetAllAsync()
    {
        return await _context.Ministries
            .Include(x => x.Leader)
            .ToListAsync();
    }

    public async Task<Ministry?> GetByIdAsync(int id)
    {
        return await _context.Ministries
            .Include(x => x.Leader)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Ministry entity)
    {
        _context.Ministries.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Ministry entity)
    {
        _context.Ministries.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Ministry entity)
    {
        _context.Ministries.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
