using IglesiaBackend.Features.ChurchFunctionRoles;
using IglesiaBackend.Features.EventRecordTypes;
using IglesiaBackend.Features.Events;
using IglesiaBackend.Features.Members;
using IglesiaBackend.Features.OrganizationMembers;     // <--- Nuevo
using IglesiaBackend.Features.OrganizationStructures;  // <--- Nuevo
using IglesiaBackend.Features.OrganizationTypes;       // <--- Nuevo
using IglesiaBackend.Features.RecordTypeFields;
using IglesiaBackend.Features.RecordTypes;
using IglesiaBackend.Features.RegistryEvents;
using IglesiaBackend.Features.Reports;
using IglesiaBackend.Features.SystemRoles;
using IglesiaBackend.Features.Users;
using IglesiaBackend.Features.UserSystemRoles;
using IglesiaBackend.Shared.Entities;
using IglesiaBackend.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;

namespace IglesiaBackend.Data;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IHttpContextAccessor httpContextAccessor
    ) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // --- ENTIDADES PRINCIPALES ---
    public DbSet<Member> Members { get; set; }
    public DbSet<User> Users { get; set; }

    // --- SEGURIDAD ---
    public DbSet<SystemRole> SystemRoles { get; set; }
    public DbSet<UserSystemRole> UserSystemRoles { get; set; }

    // --- ESTRUCTURA ORGANIZACIONAL (NUEVO NÚCLEO) ---
    public DbSet<OrganizationType> OrganizationTypes { get; set; }
    public DbSet<OrganizationStructure> OrganizationStructures { get; set; }
    public DbSet<ChurchFunctionRole> ChurchFunctionRoles { get; set; }
    public DbSet<OrganizationMember> OrganizationMembers { get; set; } // Tabla intermedia clave

    // --- EVENTOS Y FORMULARIOS ---
    public DbSet<Event> Events { get; set; }
    public DbSet<RecordType> RecordTypes { get; set; }
    public DbSet<RecordTypeField> RecordTypeFields { get; set; }
    public DbSet<EventRecordType> EventRecordTypes { get; set; }

    // --- DATA ---
    public DbSet<RegistryEvent> RegistryEvents { get; set; }
    public DbSet<Report> Reports { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================================================
        // 1. CONFIGURACIÓN SOFT DELETE (Global)
        // =========================================================
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
            }
        }

        // 🔥🔥 AGREGAR ESTO: CONFIGURACIÓN DE RELACIÓN USUARIO - MIEMBRO 🔥🔥
        modelBuilder.Entity<User>()
            .HasOne(u => u.Member)          // Un Usuario tiene un Miembro
            .WithOne(m => m.User)           // Un Miembro tiene un Usuario (Relación Inversa)
            .HasForeignKey<User>(u => u.MemberId) // La llave foránea está en la tabla USERS
            .OnDelete(DeleteBehavior.Restrict); // Evita borrados accidentales en cascada física

        // =========================================================
        // 2. CONFIGURACIÓN ESTRUCTURA ORGANIZACIONAL (Auto-Referencia)
        // =========================================================
        modelBuilder.Entity<OrganizationStructure>(entity =>
        {
            // Un Padre tiene muchos Hijos
            entity.HasOne(x => x.Parent)
                  .WithMany(x => x.Children)
                  .HasForeignKey(x => x.ParentId)
                  .OnDelete(DeleteBehavior.Restrict); // IMPORTANTE: Evita borrado en cascada accidental de ramas enteras
        });

        // Configuración adicional para OrganizationMember (opcional, EF lo deduce, pero es bueno ser explícito)
        modelBuilder.Entity<OrganizationMember>(entity =>
        {
            entity.HasOne(om => om.Member)
                  .WithMany(m => m.OrganizationMemberships)
                  .HasForeignKey(om => om.MemberId)
                  .OnDelete(DeleteBehavior.Cascade); // Si se borra el miembro, se borra su asignación
        });

        // =========================================================
        // 3. CONVERSIÓN DE ENUMS (WeekDays)
        // =========================================================
        modelBuilder.Entity<Event>()
            .Property(e => e.RecurringDays)
            .HasConversion<string>();

        // =========================================================
        // 4. MAPEO DE JSONB (PostgreSQL)
        // =========================================================
        modelBuilder.Entity<RegistryEvent>()
            .Property(e => e.DataJson)
            .HasColumnType("jsonb");

        modelBuilder.Entity<Member>()
            .Property(m => m.ExtraData)
            .HasColumnType("jsonb");

        // Configuración para guardar JsonDocument como texto en la BD
        modelBuilder.Entity<Report>()
            .Property(r => r.Configuration)
            .HasColumnType("jsonb") // <--- AGREGAR ESTO PARA POSTGRES
            .HasConversion(
                v => v.RootElement.ToString(), // Al guardar: JsonDocument -> String
                v => JsonDocument.Parse(v, default) // Al leer: String -> JsonDocument
            );

        //modelBuilder.Entity<Event>()
        //    .Property(e => e.EvidenceImages)
        //    .HasColumnType("jsonb"); // <--- IMPORTANTE
    }

    // Método auxiliar para construir la expresión Lambda dinámicamente (e => !e.IsDeleted)
    private static LambdaExpression ConvertFilterExpression(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(AuditableEntity.IsDeleted));
        var falseConstant = Expression.Constant(false);
        var equalExpression = Expression.Equal(property, falseConstant);
        return Expression.Lambda(equalExpression, parameter);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _httpContextAccessor.HttpContext?.GetUserId();
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreateAt = now;
                    entry.Entity.IdUserCreateAt = userId;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    if (!entry.Entity.IsDeleted)
                    {
                        entry.Entity.UpdateAt = now;
                        entry.Entity.IdUserUpdateAt = userId;
                    }
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = now;
                    entry.Entity.IdUserDeletedAt = userId;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}