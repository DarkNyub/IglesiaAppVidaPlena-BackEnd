using IglesiaBackend.Features.RecordTypeFields;
using IglesiaBackend.Features.RecordTypes.Dtos;

namespace IglesiaBackend.Features.RecordTypes;

public static class RecordTypeMapper
{
    public static RecordTypeDto ToDto(RecordType entity)
    {
        return new RecordTypeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsDeleted = entity.IsDeleted,

            // Mapeo de Targeting
            TargetOrganizationTypeId = entity.TargetOrganizationTypeId,
            TargetOrganizationTypeName = entity.TargetOrganizationType?.Name,

            TargetFunctionRoleId = entity.TargetFunctionRoleId,
            TargetFunctionRoleName = entity.TargetFunctionRole?.Name,

            // Usamos el Mapper de RecordTypeField para no repetir lógica
            Fields = entity.Fields
                .OrderBy(f => f.FieldOrder)
                .Select(RecordTypeFieldMapper.ToDto)
                .ToList()
        };
    }

    public static RecordType ToEntity(RecordTypeCreateUpdateDto dto)
    {
        return new RecordType
        {
            Name = dto.Name,
            Description = dto.Description,
            TargetOrganizationTypeId = dto.TargetOrganizationTypeId,
            TargetFunctionRoleId = dto.TargetFunctionRoleId,
            //IsDeleted = dto.IsDeleted,

            // Nota: Aquí creamos las entidades hijas nuevas
            Fields = dto.Fields.Select(f => new RecordTypeField
            {
                Name = f.Name,
                Label = f.Label,
                DataType = f.DataType,
                MemberSelectionLogic = f.MemberSelectionLogic,
                IsRequired = f.IsRequired,
                FieldOrder = f.FieldOrder,
                IsActive = true
            }).ToList()
        };
    }

    public static void UpdateEntity(RecordType entity, RecordTypeCreateUpdateDto dto)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.TargetOrganizationTypeId = dto.TargetOrganizationTypeId;
        entity.TargetFunctionRoleId = dto.TargetFunctionRoleId;
        //entity.IsDeleted = dto.IsDeleted;
    }
}