using IglesiaBackend.Data;
using IglesiaBackend.Features.ChurchFunctionRoles;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.OrganizationStructures;

public class OrganizationStructureRepository
{
    private readonly AppDbContext _context;

    public OrganizationStructureRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrganizationStructure>> GetAllAsync()
    {
        return await _context.OrganizationStructures
            .AsNoTracking()
            .IgnoreQueryFilters() // 🔥 IMPORTANTE: Traer también los borrados para mostrarlos en rojo
            .Include(x => x.OrganizationType) // Cargar el Tipo (Red, Ministerio...)
            .Include(x => x.Parent)           // Cargar el Padre (Jerarquía)
            .OrderBy(x => x.OrganizationTypeId) // Agrupar visualmente por tipo
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<OrganizationStructure?> GetByIdAsync(int id)
    {
        return await _context.OrganizationStructures
            .IgnoreQueryFilters() // 🔥 IMPORTANTE: Traer también los borrados para mostrarlos en rojo
            .Include(x => x.OrganizationType)
            .Include(x => x.Parent)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    // Método extra útil: Obtener hijos de una estructura (Ej: Todos los Grupos de la Red X)
    public async Task<List<OrganizationStructure>> GetChildrenAsync(int parentId)
    {
        return await _context.OrganizationStructures
            .AsNoTracking()
            .IgnoreQueryFilters() // 🔥 IMPORTANTE: Traer también los borrados para mostrarlos en rojo
            .Where(x => x.ParentId == parentId)
            .Include(x => x.OrganizationType)
            .ToListAsync();
    }

    public async Task<OrganizationStructure> CreateAsync(OrganizationStructure entity)
    {
        _context.OrganizationStructures.Add(entity);
        await _context.SaveChangesAsync();

        // Recargamos la entidad para que el DTO de retorno tenga los Includes (Type/Parent)
        await _context.Entry(entity).Reference(x => x.OrganizationType).LoadAsync();
        if (entity.ParentId != null) await _context.Entry(entity).Reference(x => x.Parent).LoadAsync();

        return entity;
    }

    public async Task UpdateAsync(OrganizationStructure entity)
    {
        _context.OrganizationStructures.Update(entity);
        await _context.SaveChangesAsync();
    }
    // Guardado ligero (útil para Restore/SoftDelete sin validaciones complejas)
    public async Task UpdateSimpleAsync(OrganizationStructure entity)
    {
        _context.OrganizationStructures.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(OrganizationStructure entity)
    {
        _context.OrganizationStructures.Remove(entity);
        await _context.SaveChangesAsync();
    }
    // Verifica si tiene sub-estructuras activas (IsDeleted = false)
    public async Task<bool> HasActiveChildrenAsync(int id)
    {
        return await _context.OrganizationStructures
            .AnyAsync(x => x.ParentId == id && !x.IsDeleted);
    }

    // Verifica si tiene miembros activos asignados.
    // (Ajusta 'MemberRoles' a la tabla que uses para relacionar Miembro-Estructura)
    public async Task<bool> HasActiveMembersAsync(int id)
    {
        // Ejemplo genérico. Si tu tabla se llama de otra forma, ajusta la consulta.
        return await _context.OrganizationMembers
            .Include(mr => mr.Member)
            .AnyAsync(mr => mr.OrganizationStructureId == id && !mr.Member.IsDeleted);
    }
    // Nuevo método para cargar el árbol completo de una estructura (útil para clonar)
    public async Task<OrganizationStructure?> GetTreeByIdAsync(int id)
    {
        // Traemos todo en memoria (sin tracking para que EF no se confunda al insertar copias)
        var allActiveNodes = await _context.OrganizationStructures
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync();

        // Buscamos la raíz solicitada
        var root = allActiveNodes.FirstOrDefault(x => x.Id == id);
        if (root == null) return null;

        // Construimos el árbol en memoria asignando los hijos
        void BuildTree(OrganizationStructure node)
        {
            node.Children = allActiveNodes.Where(x => x.ParentId == node.Id).ToList();
            foreach (var child in node.Children)
            {
                BuildTree(child);
            }
        }

        BuildTree(root);
        return root;
    }
    // ==========================================
    // MOTOR DE VISIBILIDAD DESCENDENTE (TENANCY)
    // ==========================================
    public async Task<List<int>> GetDescendingStructureIdsAsync(int memberId)
    {
        // 1. Obtener los IDs de las estructuras donde el miembro está asignado directamente
        var directStructureIds = await _context.OrganizationMembers
            .Where(om => om.MemberId == memberId && om.IsActive && !om.IsDeleted)
            .Select(om => om.OrganizationStructureId)
            .ToListAsync();

        if (!directStructureIds.Any()) return new List<int>();

        // 2. Traer la jerarquía plana a memoria (súper rápido) para evitar múltiples queries SQL
        var allStructures = await _context.OrganizationStructures
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => new { x.Id, x.ParentId })
            .ToListAsync();

        var allowedIds = new HashSet<int>(directStructureIds);
        var queue = new Queue<int>(directStructureIds);

        // 3. Algoritmo BFS (Búsqueda en Anchura) para encontrar descendientes
        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            var children = allStructures.Where(x => x.ParentId == currentId).Select(x => x.Id).ToList();
            
            foreach (var childId in children)
            {
                if (allowedIds.Add(childId)) // Si es un ID nuevo, lo agregamos y buscamos a sus hijos
                {
                    queue.Enqueue(childId);
                }
            }
        }

        return allowedIds.ToList();
    }
}