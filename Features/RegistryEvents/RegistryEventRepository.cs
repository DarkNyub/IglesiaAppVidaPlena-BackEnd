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

    // =========================================================
    // GET ALL CON SEGURIDAD JERÁRQUICA (ROW-LEVEL SECURITY)
    // =========================================================
    public async Task<List<RegistryEvent>> GetAllAsync(string userRole, int? currentMemberId)
    {
        var query = _context.RegistryEvents
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Include(r => r.Event)
            .Include(r => r.RecordType)
            .Include(r => r.Leader)
            .AsQueryable();

        var normalizedRole = userRole.ToUpper();
        bool isSuperAdmin = normalizedRole == "SUPERADMIN" || normalizedRole == "ADMIN";

        // Si NO es SuperAdmin, armamos su ramaje de permisos
        if (!isSuperAdmin && currentMemberId.HasValue)
        {
            var userStructures = await _context.OrganizationMembers
                .Where(om => om.MemberId == currentMemberId.Value)
                .Select(om => om.OrganizationStructureId)
                .ToListAsync();

            List<int> allowedStructureIds = new List<int>(userStructures);
            var allStructs = await _context.OrganizationStructures.AsNoTracking().ToListAsync();

            // Función recursiva para mapear todo el ramaje hacia abajo (Células hijas)
            void GetChildrenStructures(int parentId)
            {
                var children = allStructs.Where(s => s.ParentId == parentId).Select(s => s.Id).ToList();
                foreach (var child in children)
                {
                    if (!allowedStructureIds.Contains(child))
                    {
                        allowedStructureIds.Add(child);
                        GetChildrenStructures(child);
                    }
                }
            }

            // Alimentamos el árbol
            foreach (var us in userStructures)
            {
                GetChildrenStructures(us);
            }

            if (!allowedStructureIds.Any())
            {
                // Si el usuario no tiene ninguna red asignada, solo puede ver lo que él mismo llenó
                query = query.Where(r => r.LeaderId == currentMemberId.Value);
            }
            else
            {
                // Extraemos a todos los líderes que pertenecen a la jurisdicción de este usuario
                var validLeaderIds = await _context.OrganizationMembers
                    .Where(om => allowedStructureIds.Contains(om.OrganizationStructureId))
                    .Select(om => om.MemberId)
                    .Distinct()
                    .ToListAsync();

                // Trae el reporte SI:
                // 1. El evento pertenece explícitamente a su ramaje
                // 2. O el reporte fue llenado por alguien de su ramaje (ej. eventos globales)
                // 3. O él mismo lo llenó
                query = query.Where(x => 
                    (x.Event != null && x.Event.OrganizationStructureId.HasValue && allowedStructureIds.Contains(x.Event.OrganizationStructureId.Value)) ||
                    validLeaderIds.Contains(x.LeaderId) ||
                    x.LeaderId == currentMemberId.Value
                );
            }
        }

        return await query.OrderByDescending(r => r.RegistryDate).ToListAsync();
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
    // 🔥 NUEVO: Verifica si el líder ya envió un reporte para este evento HOY
    public async Task<bool> HasLeaderSubmittedTodayAsync(int eventId, int leaderId)
    {
        var today = DateTime.UtcNow.Date;
        
        return await _context.RegistryEvents
            .AnyAsync(r => 
                r.EventId == eventId && 
                r.LeaderId == leaderId && 
                !r.IsDeleted &&
                r.RegistryDate.Date == today);
    }

}