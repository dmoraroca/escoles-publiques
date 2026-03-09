using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <summary>
/// Encapsulates the functional responsibility of add school id to enrollments within the application architecture.
/// </summary>
[Migration("20260213131000_AddSchoolIdToEnrollments")]
public partial class AddSchoolIdToEnrollments : Migration
{
    /// <summary>
    /// Executes the up operation as part of this component.
    /// </summary>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Make the migration resilient (safe to run even if the column/constraint already exists).
        // This also allows an emergency manual SQL fix without breaking future deploys.
        migrationBuilder.Sql(@"
ALTER TABLE public.enrollments
    ADD COLUMN IF NOT EXISTS school_id bigint NULL;

UPDATE public.enrollments e
SET school_id = s.school_id
FROM public.students s
WHERE e.student_id = s.id
  AND e.school_id IS NULL;

ALTER TABLE public.enrollments
    ALTER COLUMN school_id SET NOT NULL;

CREATE INDEX IF NOT EXISTS ix_enrollments_school_id ON public.enrollments(school_id);

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_enrollments_schools_school_id') THEN
        ALTER TABLE public.enrollments
            ADD CONSTRAINT fk_enrollments_schools_school_id
            FOREIGN KEY (school_id) REFERENCES public.schools(id)
            ON DELETE RESTRICT;
    END IF;
END $$;
");
    }
    /// <summary>
    /// Executes the down operation as part of this component.
    /// </summary>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Best-effort rollback.
        migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_enrollments_schools_school_id') THEN
        ALTER TABLE public.enrollments DROP CONSTRAINT fk_enrollments_schools_school_id;
    END IF;
END $$;

DROP INDEX IF EXISTS public.ix_enrollments_school_id;

ALTER TABLE public.enrollments DROP COLUMN IF EXISTS school_id;
");
    }
}
