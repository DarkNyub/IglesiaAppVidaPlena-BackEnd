using IglesiaBackend.Features.Auth;
using IglesiaBackend.Features.ChurchFunctionRoles;
using IglesiaBackend.Features.EventRecordTypes;
using IglesiaBackend.Features.Events;
using IglesiaBackend.Features.Members;
using IglesiaBackend.Features.OrganizationMembers;
using IglesiaBackend.Features.OrganizationStructures;
using IglesiaBackend.Features.OrganizationTypes;
using IglesiaBackend.Features.RecordTypeFields;
using IglesiaBackend.Features.RecordTypes;
using IglesiaBackend.Features.RegistryEvents;
using IglesiaBackend.Features.Reports;
using IglesiaBackend.Features.SystemRoles;
using IglesiaBackend.Features.Users;
using IglesiaBackend.Features.UserSystemRoles;

namespace IglesiaBackend.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFeatureServices(this IServiceCollection services)
    {
        // 1. Auth & Core
        services.AddScoped<JwtService>();

        // 2. Users & Roles del Sistema
        services.AddScoped<UserRepository>();
        services.AddScoped<UserService>();

        services.AddScoped<SystemRoleRepository>();
        services.AddScoped<SystemRoleService>();

        services.AddScoped<UserSystemRoleRepository>();
        services.AddScoped<UserSystemRoleService>();

        // 3. Miembros & Estructura Eclesiástica
        services.AddScoped<MemberRepository>();
        services.AddScoped<MemberService>();

        services.AddScoped<ChurchFunctionRoleRepository>();
        services.AddScoped<ChurchFunctionRoleService>();

        // --- NUEVA ARQUITECTURA ORGANIZACIONAL ---
        services.AddScoped<OrganizationTypeRepository>();
        services.AddScoped<OrganizationTypeService>();

        services.AddScoped<OrganizationStructureRepository>();
        services.AddScoped<OrganizationStructureService>();

        services.AddScoped<OrganizationMemberRepository>();
        services.AddScoped<OrganizationMemberService>();

        // 4. Eventos & Formularios
        services.AddScoped<EventRepository>();
        services.AddScoped<EventService>();

        services.AddScoped<RecordTypeRepository>();
        services.AddScoped<RecordTypeService>();

        services.AddScoped<EventRecordTypeRepository>();
        services.AddScoped<EventRecordTypeService>();

        services.AddScoped<RecordTypeFieldRepository>();
        services.AddScoped<RecordTypeFieldService>();

        services.AddScoped<RegistryEventRepository>();
        services.AddScoped<RegistryEventService>();

        services.AddScoped<ReportRepository>();
        services.AddScoped<ReportService>();

        return services;
    }
}