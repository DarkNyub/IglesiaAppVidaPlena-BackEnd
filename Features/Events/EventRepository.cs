using IglesiaBackend.Data;
using IglesiaBackend.Features.EventRecordTypes;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.Events;

public class EventRepository
{
    private readonly AppDbContext _context;

    public EventRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Event>> GetAllAsync()
    {
        return await _context.Events
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.OrganizationStructure)
            .Include(x => x.EventRecordTypes)
            .ToListAsync();
    }

    public async Task<Event?> GetByIdAsync(int id)
    {
        return await _context.Events
            .IgnoreQueryFilters()
            .Include(x => x.OrganizationStructure)
            .Include(x => x.EventRecordTypes)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Event> CreateAsync(Event entity, List<int> recordTypeIds)
    {
        _context.Events.Add(entity);
        await _context.SaveChangesAsync();

        if (recordTypeIds != null && recordTypeIds.Any())
        {
            foreach (var typeId in recordTypeIds)
            {
                _context.EventRecordTypes.Add(new EventRecordType
                {
                    EventId = entity.Id,
                    RecordTypeId = typeId
                });
            }
            await _context.SaveChangesAsync();
        }
        return entity;
    }

    // 🔥 SMART SYNC: Evita duplicados y recicla Soft Deletes
    public async Task UpdateAsync(Event entity, List<int> recordTypeIds)
    {
        _context.Events.Update(entity);

        // 1. Buscar TODAS las relaciones existentes (incluyendo borradas)
        var existingRelations = await _context.EventRecordTypes
            .IgnoreQueryFilters()
            .Where(x => x.EventId == entity.Id)
            .ToListAsync();

        var existingTypeIds = existingRelations.Select(x => x.RecordTypeId).ToList();
        var newTypeIds = recordTypeIds ?? new List<int>();

        // 2. Eliminar (Soft Delete mágico) las que ya no vienen marcadas
        var toDelete = existingRelations.Where(x => !newTypeIds.Contains(x.RecordTypeId) && !x.IsDeleted).ToList();
        foreach (var item in toDelete)
        {
            _context.EventRecordTypes.Remove(item);
        }

        // 3. Restaurar las que estaban borradas y el usuario volvió a marcar
        var toRestore = existingRelations.Where(x => newTypeIds.Contains(x.RecordTypeId) && x.IsDeleted).ToList();
        foreach (var item in toRestore)
        {
            item.IsDeleted = false;
            _context.EventRecordTypes.Update(item);
        }

        // 4. Insertar ÚNICAMENTE las totalmente nuevas
        var toInsertIds = newTypeIds.Except(existingTypeIds).ToList();
        foreach (var typeId in toInsertIds)
        {
            _context.EventRecordTypes.Add(new EventRecordType
            {
                EventId = entity.Id,
                RecordTypeId = typeId
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Event entity)
    {
        _context.Events.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSimpleAsync(Event entity)
    {
        _context.Events.Update(entity);
        await _context.SaveChangesAsync();
    }
}