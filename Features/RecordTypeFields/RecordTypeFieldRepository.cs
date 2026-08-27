using IglesiaBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Features.RecordTypeFields;

public class RecordTypeFieldRepository
{
    private readonly AppDbContext _context;

    public RecordTypeFieldRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RecordTypeField>> GetByRecordTypeAsync(int recordTypeId)
        => await _context.RecordTypeFields
            .Where(x => x.RecordTypeId == recordTypeId)
            .OrderBy(x => x.FieldOrder)
            .ToListAsync();

    public async Task<RecordTypeField?> GetByIdAsync(int id)
        => await _context.RecordTypeFields.FirstOrDefaultAsync(x => x.Id == id);

    public async Task AddAsync(RecordTypeField entity)
    {
        _context.RecordTypeFields.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RecordTypeField entity)
    {
        _context.RecordTypeFields.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(RecordTypeField entity)
    {
        _context.RecordTypeFields.Remove(entity);
        await _context.SaveChangesAsync();
    }
}