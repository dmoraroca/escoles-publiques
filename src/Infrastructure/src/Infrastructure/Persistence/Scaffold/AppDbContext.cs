using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.src.Infrastructure.Persistence.Scaffold;

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
            entity.ToTable("annual_fees");

            entity.HasIndex(e => e.EnrollmentId, "IX_annual_fees_enrollment_id");

            entity.HasIndex(e => e.EnrollmentId, "ix_annual_fees_enrollment_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Currency).HasColumnName("currency");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.EnrollmentId).HasColumnName("enrollment_id");
            entity.Property(e => e.PaidAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("paid_at");
            entity.Property(e => e.PaymentRef).HasColumnName("payment_ref");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.AnnualFees)
                .HasForeignKey(d => d.EnrollmentId)
                .HasConstraintName("fk_annualfees_enrollments_enrollment_id");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("enrollments");

            entity.HasIndex(e => e.StudentId, "IX_enrollments_student_id");

            entity.HasIndex(e => e.CourseName, "idx_enrollments_course_name");

            entity.HasIndex(e => e.StudentId, "ix_enrollments_student_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcademicYear).HasColumnName("academic_year");
            entity.Property(e => e.CourseName).HasColumnName("course_name");
            entity.Property(e => e.EnrolledAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("enrolled_at");
            entity.Property(e => e.SchoolId).HasColumnName("school_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("fk_enrollments_students_student_id");
        });

        modelBuilder.Entity<School>(entity =>
        {
            entity.ToTable("schools");

            entity.HasIndex(e => e.ScopeId, "ix_schools_scope_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.City).HasColumnName("city");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsFavorite).HasColumnName("is_favorite");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.ScopeId).HasColumnName("scope_id");

            entity.HasOne(d => d.Scope).WithMany(p => p.Schools)
                .HasForeignKey(d => d.ScopeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("schools_scope_id_fkey");
        });

        modelBuilder.Entity<ScopeMnt>(entity =>
        {
            entity.ToTable("scope_mnt");

            entity.HasIndex(e => e.IsActive, "idx_scope_mnt_active");

            entity.HasIndex(e => e.Name, "idx_scope_mnt_name");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("students");

            entity.HasIndex(e => e.SchoolId, "IX_students_school_id");

            entity.HasIndex(e => e.UserId, "IX_students_user_id").IsUnique();

            entity.HasIndex(e => e.UserId, "idx_students_user_id");

            entity.HasIndex(e => e.SchoolId, "ix_students_school_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.SchoolId).HasColumnName("school_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.School).WithMany(p => p.Students)
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("fk_students_schools_school_id");

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.UserId)
                .HasConstraintName("fk_students_users_user_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "IX_users_email").IsUnique();

            entity.HasIndex(e => e.Email, "idx_users_email");

            entity.HasIndex(e => e.Role, "idx_users_role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.LastLoginAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login_at");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
