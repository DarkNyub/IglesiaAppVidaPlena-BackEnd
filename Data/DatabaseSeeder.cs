using IglesiaBackend.Features.Members;
using IglesiaBackend.Features.SystemRoles;
using IglesiaBackend.Features.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IglesiaBackend.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // =========================================
        // 1️⃣ SYSTEM ROLES (ADMIN / USER / REPORTS)
        // =========================================
        if (!await context.SystemRoles.AnyAsync())
        {
            var adminRole = new SystemRole
            {
                Name = "Administrator",
                Description = "System administrator with full access"
            };

            var userRole = new SystemRole
            {
                Name = "User",
                Description = "Standard system user"
            };

            context.SystemRoles.AddRange(adminRole, userRole);
            await context.SaveChangesAsync();
        }

        var adminSystemRole = await context.SystemRoles
            .FirstAsync(r => r.Name == "Administrator");

        // =========================================
        // 2️⃣ MEMBER (ADMIN MEMBER)
        // =========================================
        if (!await context.Members.AnyAsync())
        {
            var adminMember = new Member
            {
                FirstName = "System",
                LastName = "Administrator"
            };

            context.Members.Add(adminMember);
            await context.SaveChangesAsync();
        }

        var adminMemberDb = await context.Members.FirstAsync();

        // =========================================
        // 3️⃣ USER (ADMIN USER)
        // =========================================
        if (!await context.Users.AnyAsync())
        {
            var hasher = new PasswordHasher<User>();

            var adminUser = new User
            {
                Username = "admin",
                MemberId = adminMemberDb.Id,
                IsActive = true
            };

            adminUser.PasswordHash =
                hasher.HashPassword(adminUser, "Admin123!");

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }
    }
}
