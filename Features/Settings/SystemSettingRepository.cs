using IglesiaBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.Settings;

public class SettingsRepository
{
    private readonly AppDbContext _context;

    public SettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SystemSetting>> GetAllAsync()
    {
        return await _context.SystemSettings.ToListAsync();
    }

    public async Task<SystemSetting?> GetByKeyAsync(string key)
    {
        return await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);
    }

    public async Task UpsertAsync(string key, string value)
    {
        var existing = await GetByKeyAsync(key);
        if (existing == null)
        {
            _context.SystemSettings.Add(new SystemSetting { Key = key, Value = value });
        }
        else
        {
            existing.Value = value;
            _context.SystemSettings.Update(existing);
        }
        await _context.SaveChangesAsync();
    }
}