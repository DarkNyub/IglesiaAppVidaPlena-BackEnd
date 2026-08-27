using IglesiaBackend.Data;
using IglesiaBackend.Features; // Entidad
using IglesiaBackend.Features.Events;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.OrganizationMembers;

public class OrganizationMemberRepository
{
    private readonly AppDbContext _context;

    public OrganizationMemberRepository(AppDbContext context)
    {
        _context = context;
    }

    // Obtener TODOS (Cuidado con el performance si son miles)
    public async Task<List<OrganizationMember>> GetAllAsync()
    {
        return await _context.OrganizationMembers
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.Member)
            .Include(x => x.ChurchFunctionRole)
            .Include(x => x.OrganizationStructure)
                .ThenInclude(s => s.OrganizationType) // Include anidado para saber si es Red o Ministerio
            .OrderBy(x => x.OrganizationStructureId)
            .ToListAsync();
    }

    // Obtener por ID
    public async Task<OrganizationMember?> GetByIdAsync(int id)
    {
        return await _context.OrganizationMembers
            .IgnoreQueryFilters()
            .Include(x => x.Member)
            .Include(x => x.ChurchFunctionRole)
            .Include(x => x.OrganizationStructure)
                .ThenInclude(s => s.OrganizationType)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    // FILTRO ÚTIL 1: Obtener miembros de una Estructura específica (Ej: Todos los de la Red Jóvenes)
    public async Task<List<OrganizationMember>> GetByStructureAsync(int structureId)
    {
        return await _context.OrganizationMembers
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(x => x.OrganizationStructureId == structureId)
            .Include(x => x.Member)
            .Include(x => x.ChurchFunctionRole)
            .OrderByDescending(x => x.ChurchFunctionRole!.AuthorityLevel) // Líderes primero
            .ToListAsync();
    }

    // FILTRO ÚTIL 2: Obtener cargos de una Persona (Ej: ¿En qué está metido Juan?)
    public async Task<List<OrganizationMember>> GetByMemberAsync(int memberId)
    {
        return await _context.OrganizationMembers
            .AsNoTracking()
            .Where(x => x.MemberId == memberId)
            .Include(x => x.ChurchFunctionRole)
            .Include(x => x.OrganizationStructure)
                .ThenInclude(s => s.OrganizationType)
            .ToListAsync();
    }

    public async Task<OrganizationMember> CreateAsync(OrganizationMember entity)
    {
        _context.OrganizationMembers.Add(entity);
        await _context.SaveChangesAsync();

        // Recargar referencias para devolver DTO completo
        await _context.Entry(entity).Reference(x => x.Member).LoadAsync();
        await _context.Entry(entity).Reference(x => x.ChurchFunctionRole).LoadAsync();
        await _context.Entry(entity).Reference(x => x.OrganizationStructure).Query()
            .Include(s => s.OrganizationType).LoadAsync();

        return entity;
    }

    public async Task UpdateAsync(OrganizationMember entity)
    {
        _context.OrganizationMembers.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(OrganizationMember entity)
    {
        _context.OrganizationMembers.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSimpleAsync(OrganizationMember entity)
    {
        _context.OrganizationMembers.Update(entity);
        await _context.SaveChangesAsync();
    }
}