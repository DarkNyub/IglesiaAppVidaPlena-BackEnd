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

    public async Task<List<Event>> GetAllAsync(List<int>? allowedStructureIds = null)
    {
        var query = _context.Events
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.OrganizationStructure)
            .Include(x => x.EventRecordTypes)
            .AsQueryable();

        // 🔥 FILTRO MÁGICO DE AISLAMIENTO 🔥
        if (allowedStructureIds != null)
        {
            // Eventos globales (StructureId == null) o eventos de la red permitida
            query = query.Where(e => e.OrganizationStructureId == null || 
                                     allowedStructureIds.Contains(e.OrganizationStructureId.Value));
        }

        return await query.ToListAsync();
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
    // 🔥 NUEVO MÉTODO PARA FILTRAR FORMULARIOS SEGÚN EL ROL DEL USUARIO
    public async Task<List<IglesiaBackend.Features.RecordTypes.RecordType>> GetAllowedRecordTypesForEventAsync(int eventId, int? memberId, bool isSuperAdmin)
    {
        // 1. Traemos los formularios activos atados a este evento
        var query = _context.EventRecordTypes
            .Include(ert => ert.RecordType)
            .Where(ert => ert.EventId == eventId && !ert.IsDeleted && !ert.RecordType.IsDeleted)
            .Select(ert => ert.RecordType);

        // Si es Admin, los ve todos
        if (isSuperAdmin) return await query.ToListAsync();

        if (!memberId.HasValue) return new List<IglesiaBackend.Features.RecordTypes.RecordType>();

        // 2. Buscamos TODOS los sombreros (cargos) activos que tiene el líder
        var userMemberships = await _context.OrganizationMembers
            .Include(om => om.OrganizationStructure)
            .Where(om => om.MemberId == memberId.Value && om.IsActive && !om.IsDeleted)
            .ToListAsync();

        var userRoleIds = userMemberships.Select(om => om.ChurchFunctionRoleId).Distinct().ToList();
        var userOrgTypeIds = userMemberships
            .Where(om => om.OrganizationStructure != null)
            .Select(om => om.OrganizationStructure.OrganizationTypeId)
            .Distinct().ToList();

        var allEventRecordTypes = await query.ToListAsync();
        var allowedRecordTypes = new List<IglesiaBackend.Features.RecordTypes.RecordType>();

        // 3. Evaluamos uno por uno si el líder tiene permiso para llenarlo
        foreach(var rt in allEventRecordTypes)
        {
            bool roleMatch = rt.TargetFunctionRoleId == null || userRoleIds.Contains(rt.TargetFunctionRoleId.Value);
            bool orgTypeMatch = rt.TargetOrganizationTypeId == null || userOrgTypeIds.Contains(rt.TargetOrganizationTypeId.Value);

            if (roleMatch && orgTypeMatch)
            {
                allowedRecordTypes.Add(rt);
            }
        }

        return allowedRecordTypes;
    }
}