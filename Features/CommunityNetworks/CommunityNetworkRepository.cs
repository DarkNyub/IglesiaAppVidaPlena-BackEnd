using IglesiaBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.CommunityNetworks;

public class CommunityNetworkRepository
{
    private readonly AppDbContext _context;

    public CommunityNetworkRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CommunityNetwork>> GetAllAsync()
    {
        return await _context.CommunityNetworks
            .Include(x => x.Leader)
            .ToListAsync();
    }

    public async Task<CommunityNetwork?> GetByIdAsync(int id)
    {
        return await _context.CommunityNetworks
            .Include(x => x.Leader)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(CommunityNetwork entity)
    {
        _context.CommunityNetworks.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CommunityNetwork entity)
    {
        _context.CommunityNetworks.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(CommunityNetwork entity)
    {
        _context.CommunityNetworks.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
