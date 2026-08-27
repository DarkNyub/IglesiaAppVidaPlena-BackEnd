using IglesiaBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.RegistryEvents;

/// <summary>
/// Data access for RegistryEvent
/// </summary>
public class RegistryEventRepository
{
    private readonly AppDbContext _context;

    public RegistryEventRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RegistryEvent>> GetAllAsync()
    {
        return await _context.RegistryEvents
            .AsNoTracking()
            .IgnoreQueryFilters() // 🔥 IMPORTANTE
            .Include(r => r.Event)      // <--- IMPORTANTE
            .Include(r => r.RecordType) // <--- IMPORTANTE
            .Include(r => r.Leader)     // <--- IMPORTANTE
            .OrderByDescending(r => r.RegistryDate)
            .ToListAsync();
    }

    // FILTRO CRUCIAL: Ver registros de un evento
    public async Task<List<RegistryEvent>> GetByEventAsync(int eventId)
    {
        return await _context.RegistryEvents
            .AsNoTracking()
            .IgnoreQueryFilters() // 🔥 IMPORTANTE
            .Where(r => r.EventId == eventId)
            .Include(r => r.Event)
            .Include(r => r.RecordType)
            .Include(r => r.Leader)
            .OrderByDescending(r => r.RegistryDate)
            .ToListAsync();
    }

    public async Task<RegistryEvent?> GetByIdAsync(int id)
    {
        return await _context.RegistryEvents
            .Include(r => r.Event)
            .Include(r => r.RecordType)
            .Include(r => r.Leader)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<RegistryEvent> AddAsync(RegistryEvent entity)
    {
        _context.RegistryEvents.Add(entity);
        await _context.SaveChangesAsync();

        // Recargar para devolver DTO completo con nombres
        await _context.Entry(entity).Reference(x => x.Event).LoadAsync();
        await _context.Entry(entity).Reference(x => x.RecordType).LoadAsync();
        await _context.Entry(entity).Reference(x => x.Leader).LoadAsync();

        return entity;
    }

    public async Task UpdateAsync(RegistryEvent entity)
    {
        _context.RegistryEvents.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(RegistryEvent entity)
    {
        _context.RegistryEvents.Remove(entity);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateSimpleAsync(RegistryEvent entity)
    {
        _context.RegistryEvents.Update(entity);
        await _context.SaveChangesAsync();
    }

}