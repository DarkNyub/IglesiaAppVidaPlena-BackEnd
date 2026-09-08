namespace IglesiaBackend.Features.RecordTypeFields;

public class RecordTypeFieldService
{
    private readonly RecordTypeFieldRepository _repository;

    // AGREGAMOS "member_selection" A LA LISTA BLANCA
    private static readonly string[] AllowedDataTypes =
        { "string", "int", "decimal", "bool", "date", "image_gallery", "member_selection", "formula" };

    public RecordTypeFieldService(RecordTypeFieldRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<RecordTypeFieldDto>> GetByRecordTypeAsync(int recordTypeId)
    {
        var list = await _repository.GetByRecordTypeAsync(recordTypeId);
        return list.Select(RecordTypeFieldMapper.ToDto).ToList();
    }

    public async Task<RecordTypeFieldDto> CreateAsync(
        RecordTypeFieldCreateUpdateDto dto)
    {
        Validate(dto);

        var entity = new RecordTypeField();

        RecordTypeFieldMapper.UpdateEntity(entity, dto);
        await _repository.AddAsync(entity);

        return RecordTypeFieldMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(
        int id,
        RecordTypeFieldCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        Validate(dto);

        RecordTypeFieldMapper.UpdateEntity(entity, dto);

        await _repository.UpdateAsync(entity);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        await _repository.DeleteAsync(entity);
        return true;
    }

    private static void Validate(RecordTypeFieldCreateUpdateDto dto)
    {
        if (!AllowedDataTypes.Contains(dto.DataType.ToLower()))
            throw new ArgumentException(
                $"Tipo de dato inválido. Permitidos: {string.Join(", ", AllowedDataTypes)}");
    }
}