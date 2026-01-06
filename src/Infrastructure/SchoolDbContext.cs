using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context for the school management system. Manages entity sets and configures schema mappings for PostgreSQL.
/// </summary>
public class SchoolDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the SchoolDbContext class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the set of schools.
    /// </summary>
    public DbSet<School> Schools => Set<School>();
    /// <summary>
    /// Gets the set of students.
    /// </summary>
    public DbSet<Student> Students => Set<Student>();
    /// <summary>
    /// Gets the set of enrollments.
    /// </summary>
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    /// <summary>
    /// Gets the set of annual fees.
    /// </summary>
    public DbSet<AnnualFee> AnnualFees => Set<AnnualFee>();
    /// <summary>
    /// Gets the set of scopes.
    /// </summary>
    public DbSet<Scope> Scopes => Set<Scope>();
    /// <summary>
    /// Gets the set of users.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Configures the entity mappings and schema for the database, including table and column naming conventions and specific table configurations.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar PostgreSQL per usar timestamps sense timezone
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

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
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasIndex(e => e.Email).IsUnique();
        });
        // Aplica totes les configuracions Fluent API
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchoolDbContext).Assembly);
    }

    /// <summary>
    /// Converts a string from PascalCase or camelCase to snake_case.
    /// </summary>
    /// <param name="name">The string to convert.</param>
    /// <returns>The converted snake_case string.</returns>
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
