using IglesiaBackend.Features.Reports.Dtos;

namespace IglesiaBackend.Features.Reports;

public static class ReportMapper
{
    public static ReportDto ToDto(Report entity)
    {
        return new ReportDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            TypeReport = entity.TypeReport,
            Configuration = entity.Configuration,
            // 🔥 AGREGAR MAPEO
            IsDeleted = entity.IsDeleted
        };
    }

    public static Report ToEntity(ReportCreateUpdateDto dto)
    {
        return new Report
        {
            Name = dto.Name,
            Description = dto.Description,
            TypeReport = dto.TypeReport,
            Configuration = dto.Configuration
        };
    }

    public static void UpdateEntity(Report entity, ReportCreateUpdateDto dto)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.TypeReport = dto.TypeReport;
        entity.Configuration = dto.Configuration;
    }
}