using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;
/// <summary>
/// Encapsulates the functional responsibility of app db context factory within the application architecture.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<SchoolDbContext>
{
    /// <summary>
    /// Creates db context by applying the required business rules.
    /// </summary>
    public SchoolDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SchoolDbContext>();

        // Mateixa conn string que estàs fent servir
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=escoles;Username=app;Password=app");

        return new SchoolDbContext(optionsBuilder.Options);
    }
}