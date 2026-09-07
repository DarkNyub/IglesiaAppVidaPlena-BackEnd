using IglesiaBackend.Data;
using IglesiaBackend.Features.OrganizationMembers;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.Members;

public class MemberRepository
{
    private readonly AppDbContext _context;

    public MemberRepository(AppDbContext context)
    {
        _context = context;
    }
    
    // =========================================================
    // GET ALL CON SEGURIDAD JERÁRQUICA (ROW-LEVEL SECURITY)
    // =========================================================
    public async Task<List<Member>> GetAllAsync(string userRole, int? currentMemberId)
    {
        var query = _context.Members
            .IgnoreQueryFilters() 
            .AsNoTracking()
            .AsSplitQuery() 
            .Include(x => x.User) 
            .Include(x => x.OrganizationMemberships)
                .ThenInclude(om => om.OrganizationStructure) 
            .Include(x => x.OrganizationMemberships)
                .ThenInclude(om => om.ChurchFunctionRole)    
            .AsQueryable();

        var normalizedRole = userRole.ToUpper();
        bool isSuperAdmin = normalizedRole == "SUPERADMIN" || normalizedRole == "ADMIN";

        // Si NO es SuperAdmin, aplicamos el filtro jerárquico
        if (!isSuperAdmin && currentMemberId.HasValue)
        {
            // 1. Obtener las estructuras directas del usuario logueado
            var userStructures = await _context.OrganizationMembers
                .Where(om => om.MemberId == currentMemberId.Value)
                .Select(om => om.OrganizationStructureId)
                .ToListAsync();

            List<int> allowedStructureIds = new List<int>(userStructures);
            var allStructs = await _context.OrganizationStructures.AsNoTracking().ToListAsync();

            // 2. Función recursiva para mapear todo el ramaje hacia abajo (Células hijas)
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

            // Alimentamos el árbol con las redes del líder
            foreach (var us in userStructures)
            {
                GetChildrenStructures(us);
            }

            // 3. Aplicamos el filtro a la consulta SQL
            if (!allowedStructureIds.Any())
            {
                // Si el usuario no tiene ninguna red asignada, solo puede verse a sí mismo
                query = query.Where(m => m.Id == currentMemberId.Value);
            }
            else
            {
                // Solo trae a los miembros que tengan un cargo en las redes permitidas, 
                // o al propio usuario logueado.
                query = query.Where(m => 
                    m.Id == currentMemberId.Value || 
                    m.OrganizationMemberships.Any(om => allowedStructureIds.Contains(om.OrganizationStructureId))
                );
            }
        }

        return await query.ToListAsync();
    }

    public async Task<Member?> GetByIdAsync(int id)
    {
        return await _context.Members
            .IgnoreQueryFilters()
            .AsSplitQuery() // <--- AGREGA ESTO
            .Include(x => x.User)
            .Include(x => x.OrganizationMemberships) // <--- CARGAMOS ROLES AQUÍ
                .ThenInclude(om => om.OrganizationStructure)
            .Include(x => x.OrganizationMemberships)
                .ThenInclude(om => om.ChurchFunctionRole)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    // En el CreateAsync también debes agregar los roles:
    public async Task<Member> CreateAsync(Member entity, List<MemberRoleAssignmentDto> roles)
    {
        _context.Members.Add(entity);

        // Guardar primero para tener el ID del miembro
        await _context.SaveChangesAsync();

        // Agregar Roles
        if (roles.Any())
        {
            foreach (var r in roles)
            {
                _context.OrganizationMembers.Add(new OrganizationMember
                {
                    MemberId = entity.Id,
                    OrganizationStructureId = r.StructureId,
                    ChurchFunctionRoleId = r.RoleId,
                    IsActive = true
                });
            }
            await _context.SaveChangesAsync();
        }

        return entity;
    }

    public async Task UpdateAsync(Member entity, List<MemberRoleAssignmentDto> newRoles)
    {
        // 1. Actualizar datos básicos del miembro
        _context.Members.Update(entity);

        // 2. SINCRONIZACIÓN INTELIGENTE DE ROLES (SMART SYNC)

        // A) Traemos TODOS los roles que existen en BD para este miembro (Activos y Borrados)
        var currentDbRoles = await _context.OrganizationMembers
            .IgnoreQueryFilters() // <--- CLAVE: Para ver los que están SoftDeleted
            .Where(x => x.MemberId == entity.Id)
            .ToListAsync();

        // B) Procesamos la lista nueva que llega del Front
        foreach (var newRole in newRoles)
        {
            // Buscamos si ya existe ese par (Estructura + Rol) en la BD
            var existingRole = currentDbRoles.FirstOrDefault(x =>
                x.OrganizationStructureId == newRole.StructureId &&
                x.ChurchFunctionRoleId == newRole.RoleId);

            if (existingRole != null)
            {
                // CASO 1: YA EXISTE (Reciclamos)
                // Si estaba borrado o inactivo, lo reactivamos.
                if (existingRole.IsDeleted || !existingRole.IsActive)
                {
                    existingRole.IsDeleted = false;
                    existingRole.IsActive = true;
                    // Opcional: existingRole.LastModifiedDate = DateTime.UtcNow;
                    _context.OrganizationMembers.Update(existingRole);
                }
                // Si ya estaba activo, no hacemos nada (ahorramos query).
            }
            else
            {
                // CASO 2: ES NUEVO (Insertamos)
                _context.OrganizationMembers.Add(new OrganizationMember
                {
                    MemberId = entity.Id,
                    OrganizationStructureId = newRole.StructureId,
                    ChurchFunctionRoleId = newRole.RoleId,
                    IsActive = true,
                    IsDeleted = false
                });
            }
        }

        // C) Borrar (Soft Delete) los que están en BD pero NO vinieron en la lista nueva
        // Identificamos cuáles de la BD NO están en la lista newRoles
        var rolesToDelete = currentDbRoles.Where(dbRole =>
            !newRoles.Any(newR =>
                newR.StructureId == dbRole.OrganizationStructureId &&
                newR.RoleId == dbRole.ChurchFunctionRoleId
            ))
            .ToList();

        foreach (var role in rolesToDelete)
        {
            // Solo marcamos como borrado si no lo estaba ya
            if (!role.IsDeleted)
            {
                role.IsDeleted = true;
                // role.DeletedDate = DateTime.UtcNow; // Si usas auditoría manual
                _context.OrganizationMembers.Remove(role);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Member entity)
    {
        _context.Members.Remove(entity);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateSimpleAsync(Member entity)
    {
        _context.Members.Update(entity);
        // Al no tocar la colección 'OrganizationMemberships', EF Core no hará nada con ella.
        await _context.SaveChangesAsync();
    }
    public async Task<int> BulkCreateAsync(List<Member> entities)
    {
        _context.Members.AddRange(entities);
        await _context.SaveChangesAsync(); // Se guardan todos los miembros y sus roles en un solo viaje
        return entities.Count;
    }
    // ==========================================
    // METODO PARA CARGA MASIVA (Búsqueda por Cédula)
    // ==========================================
    public async Task<Member?> GetByDocumentAsync(string document)
    {
        return await _context.Members
            .IgnoreQueryFilters() // 🔥 VITAL: Buscar incluso si están dados de baja
            .Include(x => x.OrganizationMemberships) // Traemos los roles actuales para el Smart Sync
            .FirstOrDefaultAsync(x => x.Document == document);
    }
}