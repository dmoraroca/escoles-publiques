using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class SchoolDbContext : DbContext
{
    public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
        : base(options)
    {
    }

    // DbSets (taules)
    // public DbSet<School> Schools => Set<School>();
    // public DbSet<Student> Students => Set<Student>();
    // public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    // public DbSet<AnnualFee> AnnualFees => Set<AnnualFee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica totes les configuracions Fluent API
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchoolDbContext).Assembly);
    }
}
