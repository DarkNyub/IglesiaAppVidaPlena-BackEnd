namespace IglesiaBackend.Features.MemberChurchFunctionRoles;

public static class MemberChurchFunctionRoleMapper
{
    public static MemberChurchFunctionRoleDto ToDto(
        MemberChurchFunctionRole entity)
    {
        return new MemberChurchFunctionRoleDto
        {
            Id = entity.Id,
            MemberId = entity.MemberId,
            MemberFullName = entity.Member != null
                ? $"{entity.Member.FirstName} {entity.Member.LastName}"
                : null,
            ChurchFunctionRoleId = entity.ChurchFunctionRoleId,
            ChurchFunctionRoleName = entity.ChurchFunctionRole?.Name
        };
    }

    public static void MapCreateUpdateDto(
        MemberChurchFunctionRoleCreateUpdateDto dto,
        MemberChurchFunctionRole entity)
    {
        entity.MemberId = dto.MemberId;
        entity.ChurchFunctionRoleId = dto.ChurchFunctionRoleId;
    }
}
