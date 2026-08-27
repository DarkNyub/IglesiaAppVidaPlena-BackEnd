using IglesiaBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.EventRecordTypes;

public class EventRecordTypeRepository
{
    private readonly AppDbContext _context;

    public EventRecordTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EventRecordType>> GetAllAsync()
    {
        return await _context.EventRecordTypes
            .AsNoTracking()
            .Include(x => x.RecordType) // <--- CRUCIAL: Para saber el nombre del formulario
            .ToListAsync();
    }

    // Método útil extra: Obtener por Evento
    public async Task<List<EventRecordType>> GetByEventAsync(int eventId)
    {
        return await _context.EventRecordTypes
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .Include(x => x.RecordType)
            .ToListAsync();
    }

    public async Task<EventRecordType?> GetByIdAsync(int id)
    {
        return await _context.EventRecordTypes
            .Include(x => x.RecordType)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsAsync(int eventId, int recordTypeId)
    {
        return await _context.EventRecordTypes
            .AnyAsync(x => x.EventId == eventId && x.RecordTypeId == recordTypeId);
    }

    public async Task AddAsync(EventRecordType entity)
    {
        _context.EventRecordTypes.Add(entity);
        await _context.SaveChangesAsync();

        // Recargar referencia para devolver nombre correcto
        await _context.Entry(entity).Reference(x => x.RecordType).LoadAsync();
    }

    public async Task DeleteAsync(EventRecordType entity)
    {
        _context.EventRecordTypes.Remove(entity);
        await _context.SaveChangesAsync();
    }
}