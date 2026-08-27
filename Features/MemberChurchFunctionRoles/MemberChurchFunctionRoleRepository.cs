using IglesiaBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.MemberChurchFunctionRoles;

public class MemberChurchFunctionRoleRepository
{
    private readonly AppDbContext _context;

    public MemberChurchFunctionRoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MemberChurchFunctionRole>> GetAllAsync()
    {
        return await _context.MemberChurchFunctionRoles
            .Include(x => x.Member)
            .Include(x => x.ChurchFunctionRole)
            .ToListAsync();
    }

    public async Task<MemberChurchFunctionRole?> GetByIdAsync(int id)
    {
        return await _context.MemberChurchFunctionRoles
            .Include(x => x.Member)
            .Include(x => x.ChurchFunctionRole)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(MemberChurchFunctionRole entity)
    {
        _context.MemberChurchFunctionRoles.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MemberChurchFunctionRole entity)
    {
        _context.MemberChurchFunctionRoles.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(MemberChurchFunctionRole entity)
    {
        _context.MemberChurchFunctionRoles.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
