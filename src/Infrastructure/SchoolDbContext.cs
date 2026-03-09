using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// Encapsulates the Entity Framework DbContext used by the persistence layer.
/// </summary>
public class SchoolDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SchoolDbContext"/> class.
    /// </summary>
    /// <param name="options">Configuration options for the DbContext instance.</param>
    public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the schools set.
    /// </summary>
    public DbSet<School> Schools => Set<School>();

    /// <summary>
    /// Gets the students set.
    /// </summary>
    public DbSet<Student> Students => Set<Student>();

    /// <summary>
    /// Gets the enrollments set.
    /// </summary>
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    /// <summary>
    /// Gets the annual fees set.
    /// </summary>
    public DbSet<AnnualFee> AnnualFees => Set<AnnualFee>();

    /// <summary>
    /// Gets the scopes set.
    /// </summary>
    public DbSet<Scope> Scopes => Set<Scope>();

    /// <summary>
    /// Gets the users set.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Configures entity mappings and naming conventions for PostgreSQL.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure PostgreSQL to use timestamps without timezone.
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // Apply global snake_case conventions for tables, columns, and foreign keys.
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()?.ToLowerInvariant());

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }

            foreach (var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(key.GetConstraintName()?.ToLowerInvariant());
            }
        }

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

        // Apply all Fluent API configurations found in this assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchoolDbContext).Assembly);
    }

    /// <summary>
    /// Converts a PascalCase identifier to snake_case.
    /// </summary>
    private static string ToSnakeCase(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

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
