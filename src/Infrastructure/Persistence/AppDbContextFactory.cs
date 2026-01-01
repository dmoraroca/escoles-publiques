using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<SchoolDbContext>
{
    public SchoolDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SchoolDbContext>();

        // Mateixa conn string que estàs fent servir
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=escoles;Username=app;Password=app");

        return new SchoolDbContext(optionsBuilder.Options);
    }
}