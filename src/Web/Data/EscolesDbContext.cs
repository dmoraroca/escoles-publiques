using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Web.Models.Scaffold;

namespace Web.Data;

public partial class EscolesDbContext : DbContext
{
    public EscolesDbContext(DbContextOptions<EscolesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<annual_fee> annual_fees { get; set; }

    public virtual DbSet<enrollment> enrollments { get; set; }

    public virtual DbSet<school> schools { get; set; }

    public virtual DbSet<scope_mnt> scope_mnts { get; set; }

    public virtual DbSet<student> students { get; set; }

    public virtual DbSet<user> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<annual_fee>(entity =>
        {
            entity.HasIndex(e => e.enrollment_id, "IX_annual_fees_enrollment_id");

            entity.HasIndex(e => e.enrollment_id, "ix_annual_fees_enrollment_id");

            entity.Property(e => e.paid_at).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.enrollment).WithMany(p => p.annual_fees)
                .HasForeignKey(d => d.enrollment_id)
                .HasConstraintName("fk_annualfees_enrollments_enrollment_id");
        });

        modelBuilder.Entity<enrollment>(entity =>
        {
            entity.HasIndex(e => e.student_id, "IX_enrollments_student_id");

            entity.HasIndex(e => e.course_name, "idx_enrollments_course_name");

            entity.HasIndex(e => e.student_id, "ix_enrollments_student_id");

            entity.Property(e => e.enrolled_at).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.student).WithMany(p => p.enrollments)
                .HasForeignKey(d => d.student_id)
                .HasConstraintName("fk_enrollments_students_student_id");
        });

        modelBuilder.Entity<school>(entity =>
        {
            entity.HasIndex(e => e.scope_id, "ix_schools_scope_id");

            entity.Property(e => e.created_at).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.scope).WithMany(p => p.schools)
                .HasForeignKey(d => d.scope_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("schools_scope_id_fkey");
        });

        modelBuilder.Entity<scope_mnt>(entity =>
        {
            entity.ToTable("scope_mnt");

            entity.HasIndex(e => e.is_active, "idx_scope_mnt_active");

            entity.HasIndex(e => e.name, "idx_scope_mnt_name");

            entity.Property(e => e.created_at).HasColumnType("timestamp without time zone");
            entity.Property(e => e.updated_at).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<student>(entity =>
        {
            entity.HasIndex(e => e.school_id, "IX_students_school_id");

            entity.HasIndex(e => e.user_id, "IX_students_user_id").IsUnique();

            entity.HasIndex(e => e.user_id, "idx_students_user_id");

            entity.HasIndex(e => e.school_id, "ix_students_school_id");

            entity.Property(e => e.created_at).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.school).WithMany(p => p.students)
                .HasForeignKey(d => d.school_id)
                .HasConstraintName("fk_students_schools_school_id");

            entity.HasOne(d => d.user).WithOne(p => p.student)
                .HasForeignKey<student>(d => d.user_id)
                .HasConstraintName("fk_students_users_user_id");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasIndex(e => e.email, "IX_users_email").IsUnique();

            entity.HasIndex(e => e.email, "idx_users_email");

            entity.HasIndex(e => e.role, "idx_users_role");

            entity.Property(e => e.created_at).HasColumnType("timestamp without time zone");
            entity.Property(e => e.last_login_at).HasColumnType("timestamp without time zone");
            entity.Property(e => e.updated_at).HasColumnType("timestamp without time zone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
