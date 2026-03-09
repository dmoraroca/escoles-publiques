using System.Security.Cryptography;
using System.Text;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api.Services;
/// <summary>
/// Seeds the database with default scope entries and an administrator user when the database is empty.
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Seeds initial records only when no users exist, and returns whether seeding was performed.
    /// </summary>
    public static bool SeedIfEmpty(SchoolDbContext db, IConfiguration config, ILogger logger)
    {
        // "Empty" check: if there is any user, assume the DB has already been initialized.
        if (db.Users.Any()) return false;

        static string Sha256Hex(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        var adminEmail = config.GetValue<string>("Seed:AdminEmail") ?? "admin@admin.adm";
        var adminPassword = config.GetValue<string>("Seed:AdminPassword") ?? "admin123";
        var adminFirst = config.GetValue<string>("Seed:AdminFirstName") ?? "Admin";
        var adminLast = config.GetValue<string>("Seed:AdminLastName") ?? "Root";

        // Scopes (basic)
        if (!db.Scopes.Any())
        {
            db.Scopes.AddRange(
                new Scope { Name = "Infantil", Description = "Educacio infantil", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Scope { Name = "Primaria", Description = "Educacio primaria", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Scope { Name = "Secundaria", Description = "Educacio secundaria", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Scope { Name = "FP", Description = "Formacio professional", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
        }

        // Admin user
        db.Users.Add(new User
        {
            FirstName = adminFirst,
            LastName = adminLast,
            Email = adminEmail,
            PasswordHash = Sha256Hex(adminPassword),
            Role = "ADM",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        db.SaveChanges();
        logger.LogWarning("Seed completed: admin user created (email={Email}). Rotate password after first login.", adminEmail);
        return true;
    }
}
