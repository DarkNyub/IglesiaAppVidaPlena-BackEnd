using IglesiaBackend.Data;
using IglesiaBackend.Features.ChurchFunctionRoles;
using IglesiaBackend.Features.RecordTypeFields;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.RecordTypes;

public class RecordTypeRepository
{
    private readonly AppDbContext _context;

    public RecordTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RecordType>> GetAllAsync()
    {
        return await _context.RecordTypes
            .AsNoTracking()
            .IgnoreQueryFilters() // 🔥 IMPORTANTE: Traer también los borrados para mostrarlos en rojo
            .Include(rt => rt.TargetOrganizationType) // Cargar Nombre Tipo
            .Include(rt => rt.TargetFunctionRole)     // Cargar Nombre Rol
            .Include(rt => rt.Fields
            .OrderBy(f => f.FieldOrder))
            .ToListAsync();
    }

    public async Task<RecordType?> GetByIdAsync(int id)
    {
        return await _context.RecordTypes
            .IgnoreQueryFilters() // 🔥 IMPORTANTE: Traer también los borrados para mostrarlos en rojo
            .Include(rt => rt.TargetOrganizationType)
            .Include(rt => rt.TargetFunctionRole)
            .Include(rt => rt.Fields.OrderBy(f => f.FieldOrder))
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<RecordType> AddAsync(RecordType entity)
    {
        _context.RecordTypes.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(RecordType entity)
    {
        _context.RecordTypes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(RecordType entity)
    {
        _context.RecordTypes.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<RecordType?> GetByEventIdAsync(int eventId)
    {
        return await _context.RecordTypes
            .Include(rt => rt.Fields.OrderBy(f => f.FieldOrder))
            .Where(rt => rt.EventRecordTypes.Any(ert => ert.EventId == eventId))
            .FirstOrDefaultAsync();
    }
    // Guardado ligero (útil para Restore/SoftDelete sin validaciones complejas)
    public async Task UpdateSimpleAsync(RecordType entity)
    {
        _context.RecordTypes.Update(entity);
        await _context.SaveChangesAsync();
    }

    // 🔥 SMART SYNC: Sincroniza los campos y aplica auditoría
    public async Task UpdateWithFieldsAsync(RecordType entity, List<RecordTypeFieldCreateUpdateDto> incomingFields)
    {
        _context.RecordTypes.Update(entity);

        // 1. Traer todos los campos (incluso los borrados)
        var existingFields = await _context.RecordTypeFields
            .IgnoreQueryFilters()
            .Where(f => f.RecordTypeId == entity.Id)
            .ToListAsync();

        // 2. PROCESAR ENTRANTES (Update / Restore / Insert)
        foreach (var incoming in incomingFields)
        {
            var match = existingFields.FirstOrDefault(f => f.Name == incoming.Name);

            if (match != null)
            {
                // UPDATE
                match.Label = incoming.Label;
                match.DataType = incoming.DataType;
                match.MemberSelectionLogic = incoming.MemberSelectionLogic;
                match.IsRequired = incoming.IsRequired;
                match.FieldOrder = incoming.FieldOrder;
                match.IsActive = true;

                // 🔥 Si estaba borrado, lo resucitamos
                if (match.IsDeleted) match.IsDeleted = false;

                _context.RecordTypeFields.Update(match);
            }
            else
            {
                // INSERT
                _context.RecordTypeFields.Add(new RecordTypeField
                {
                    RecordTypeId = entity.Id,
                    Name = incoming.Name,
                    Label = incoming.Label,
                    DataType = incoming.DataType,
                    MemberSelectionLogic = incoming.MemberSelectionLogic,
                    IsRequired = incoming.IsRequired,
                    FieldOrder = incoming.FieldOrder,
                    IsActive = true
                });
            }
        }

        // 3. SOFT DELETE (Los campos que ya no vienen del frontend)
        var incomingNames = incomingFields.Select(i => i.Name).ToList();
        var fieldsToRetire = existingFields
            .Where(f => !incomingNames.Contains(f.Name) && !f.IsDeleted)
            .ToList();

        foreach (var field in fieldsToRetire)
        {
            field.IsActive = false;
            // 🔥 LA MAGIA: Usamos Remove() para que el DbContext intercepte 
            // y asigne IsDeleted, DeletedAt, y el IdUserDeletedAt
            _context.RecordTypeFields.Remove(field);
        }

        await _context.SaveChangesAsync();
    }
}