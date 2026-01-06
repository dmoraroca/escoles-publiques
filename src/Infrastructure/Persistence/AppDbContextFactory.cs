using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

/// <summary>
/// Factory for creating SchoolDbContext instances at design time, used for migrations and tooling.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<SchoolDbContext>
{
    /// <summary>
    /// Creates a new instance of SchoolDbContext with the configured options.
    /// </summary>
    /// <param name="args">Arguments for context creation.</param>
    /// <returns>A new SchoolDbContext instance.</returns>
    public SchoolDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SchoolDbContext>();

        // Mateixa conn string que estàs fent servir
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=escoles;Username=app;Password=app");

        return new SchoolDbContext(optionsBuilder.Options);
    }
}