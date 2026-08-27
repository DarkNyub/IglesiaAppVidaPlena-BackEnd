namespace IglesiaBackend.Features.RecordTypeFields;

public static class RecordTypeFieldMapper
{
    public static RecordTypeFieldDto ToDto(RecordTypeField entity)
    {
        return new RecordTypeFieldDto
        {
            Id = entity.Id,
            RecordTypeId = entity.RecordTypeId,
            Name = entity.Name,
            Label = entity.Label,
            DataType = entity.DataType,
            MemberSelectionLogic = entity.MemberSelectionLogic, // <--- NUEVO
            IsRequired = entity.IsRequired,
            FieldOrder = entity.FieldOrder,
            IsDeleted = entity.IsDeleted // 🔥 MAPEARLO
        };
    }

    public static void UpdateEntity(
        RecordTypeField entity,
        RecordTypeFieldCreateUpdateDto dto)
    {
        entity.RecordTypeId = dto.RecordTypeId;
        entity.Name = dto.Name;
        entity.Label = dto.Label;
        entity.DataType = dto.DataType;
        entity.MemberSelectionLogic = dto.MemberSelectionLogic; // <--- NUEVO
        entity.IsRequired = dto.IsRequired;
        entity.FieldOrder = dto.FieldOrder;
    }
}