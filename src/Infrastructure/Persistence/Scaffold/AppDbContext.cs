using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Scaffold;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnnualFee> AnnualFees { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<School> Schools { get; set; }

    public virtual DbSet<ScopeMnt> ScopeMnts { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=escoles;Username=app;Password=app");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<AnnualFee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("annual_fees_pkey");

            entity.ToTable("annual_fees");

            entity.HasIndex(e => e.EnrollmentId, "ix_annual_fees_enrollment_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .HasDefaultValueSql("'EUR'::bpchar")
                .IsFixedLength()
                .HasColumnName("currency");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.EnrollmentId).HasColumnName("enrollment_id");
            entity.Property(e => e.PaidAt).HasColumnName("paid_at");
            entity.Property(e => e.PaymentRef).HasColumnName("payment_ref");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.AnnualFees)
                .HasForeignKey(d => d.EnrollmentId)
                .HasConstraintName("annual_fees_enrollment_id_fkey");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("enrollments_pkey");

            entity.ToTable("enrollments");

            entity.HasIndex(e => new { e.StudentId, e.AcademicYear }, "enrollments_student_id_academic_year_key").IsUnique();

            entity.HasIndex(e => e.CourseName, "idx_enrollments_course_name");

            entity.HasIndex(e => e.StudentId, "ix_enrollments_student_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcademicYear).HasColumnName("academic_year");
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .HasColumnName("course_name");
            entity.Property(e => e.EnrolledAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("enrolled_at");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'Active'::text")
                .HasColumnName("status");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("enrollments_student_id_fkey");
        });

        modelBuilder.Entity<School>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("schools_pkey");

            entity.ToTable("schools");

            entity.HasIndex(e => e.IsFavorite, "idx_schools_is_favorite");

            entity.HasIndex(e => e.Scope, "idx_schools_scope");

            entity.HasIndex(e => e.Code, "schools_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.City).HasColumnName("city");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.IsFavorite)
                .HasDefaultValue(false)
                .HasComment("Indica si l'escola és marcada com a favorita")
                .HasColumnName("is_favorite");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Scope)
                .HasMaxLength(100)
                .HasComment("Àmbit de l'escola (ex: Infantil, Primària, Secundària, Batxillerat)")
                .HasColumnName("scope");
        });

        modelBuilder.Entity<ScopeMnt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("scope_mnt_pkey");

            entity.ToTable("scope_mnt", tb => tb.HasComment("Taula de manteniment dels àmbits/nivells educatius de les escoles"));

            entity.HasIndex(e => e.IsActive, "idx_scope_mnt_active");

            entity.HasIndex(e => e.Name, "idx_scope_mnt_name");

            entity.HasIndex(e => e.Name, "scope_mnt_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasComment("Identificador únic de l'àmbit")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Data de creació del registre")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasComment("Descripció de l'àmbit")
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasComment("Indica si l'àmbit està actiu")
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasComment("Nom de l'àmbit educatiu")
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Data d'última actualització")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("students_pkey");

            entity.ToTable("students");

            entity.HasIndex(e => e.UserId, "idx_students_user_id");

            entity.HasIndex(e => e.SchoolId, "ix_students_school_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.SchoolId).HasColumnName("school_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.School).WithMany(p => p.Students)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("students_school_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_students_users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "idx_users_email");

            entity.HasIndex(e => e.Role, "idx_users_role");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastLoginAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login_at");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(64)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(10)
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
