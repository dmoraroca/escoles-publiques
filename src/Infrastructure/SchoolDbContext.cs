using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class SchoolDbContext : DbContext
{
    public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
        : base(options)
    {
    }

    // DbSets (taules)
    public DbSet<School> Schools => Set<School>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<AnnualFee> AnnualFees => Set<AnnualFee>();
    public DbSet<Scope> Scopes => Set<Scope>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuració global: PostgreSQL utilitza snake_case
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Convertir noms de taules a snake_case
            entity.SetTableName(entity.GetTableName()?.ToLowerInvariant());

            // Convertir noms de columnes a snake_case
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }

            // Convertir noms de claus foranes a snake_case
            foreach (var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(key.GetConstraintName()?.ToLowerInvariant());
            }
        }

        // Configuració específica de taules
        modelBuilder.Entity<School>(entity =>
        {
            entity.ToTable("schools");
        });
        
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("students");
        });
        
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("enrollments");
        });
        
        modelBuilder.Entity<AnnualFee>(entity =>
        {
            entity.ToTable("annual_fees");
        });
        
        modelBuilder.Entity<Scope>(entity =>
        {
            entity.ToTable("scope_mnt");
        });

        // Aplica totes les configuracions Fluent API
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchoolDbContext).Assembly);
    }

    private static string ToSnakeCase(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var result = new System.Text.StringBuilder();
        result.Append(char.ToLowerInvariant(name[0]));

        for (int i = 1; i < name.Length; i++)
        {
            if (char.IsUpper(name[i]))
            {
                result.Append('_');
                result.Append(char.ToLowerInvariant(name[i]));
            }
            else
            {
                result.Append(name[i]);
            }
        }

        return result.ToString();
    }
}
