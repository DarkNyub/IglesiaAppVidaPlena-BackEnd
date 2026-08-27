using IglesiaBackend.Features.Members;
using IglesiaBackend.Features.OrganizationMembers;

namespace IglesiaBackend.Features.Members;

public static class MemberMapper
{
    public static MemberDto ToDto(Member entity)
    {
        return new MemberDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Document = entity.Document,
            Phone = entity.Phone,
            Email = entity.Email,
            Address = entity.Address,
            BirthDate = entity.BirthDate,

            // JSONB directo (Entity -> DTO)
            ExtraData = entity.ExtraData,

            IsDeleted = entity.IsDeleted, // <--- MAPEO

            // Si el repositorio hizo .Include(x => x.User), entity.User NO será null
            LinkedUser = entity.User != null ? new MemberLinkedUserDto
            {
                Id = entity.User.Id,
                Username = entity.User.Username
            } : null,

            // --- MAPEO DE ROLES DETALLADO (Para edición y lógica) ---
            Roles = entity.OrganizationMemberships?
                .Where(x => x.IsActive && !x.IsDeleted) // <--- 🔥 AGREGA !x.IsDeleted
                .Select(x => new MemberRoleDto
                {
                    OrganizationStructureId = x.OrganizationStructureId,
                    OrganizationName = x.OrganizationStructure?.Name ?? "Desconocido",
                    ChurchFunctionRoleId = x.ChurchFunctionRoleId,
                    RoleName = x.ChurchFunctionRole?.Name ?? "Desconocido"
                })
                .ToList() ?? new List<MemberRoleDto>(),

            // --- RESUMEN DE TEXTO (Para mostrar rápido en tablas) ---
            RolesSummary = entity.OrganizationMemberships?
                .Where(x => x.IsActive && !x.IsDeleted) // <--- 🔥 AGREGA !x.IsDeleted AQUÍ TAMBIÉN
                .Select(x => $"{x.ChurchFunctionRole?.Name ?? "Rol"} en {x.OrganizationStructure?.Name ?? "Estructura"}")
                .ToList() ?? new List<string>()
        };
    }

    public static Member ToEntity(MemberCreateUpdateDto dto)
    {
        // Nota: Los Roles se procesan en el Repositorio/Servicio, no aquí,
        // porque requieren acceso al DbContext para crear las relaciones.
        return new Member
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Document = dto.Document,

            // CORREGIDO: Phone
            Phone = dto.Phone,

            Email = dto.Email,
            Address = dto.Address,
            BirthDate = dto.BirthDate,

            // JSONB directo
            ExtraData = dto.ExtraData,

        };
    }

    public static void UpdateEntity(Member entity, MemberCreateUpdateDto dto)
    {
        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.Document = dto.Document;

        // CORREGIDO: Actualización de Phone
        entity.Phone = dto.Phone;

        entity.Email = dto.Email;
        entity.Address = dto.Address;
        entity.BirthDate = dto.BirthDate;

        // Actualización del JSONB completo
        entity.ExtraData = dto.ExtraData;

        //entity.IsDeleted = dto.IsDeleted; // <--- ACTUALIZACIÓN
    }
}