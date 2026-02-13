using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <summary>
/// Adds the missing enrollments.school_id column expected by the domain model and API.
/// This fixes production 500s when querying /api/enrollments.
/// </summary>
[Migration("20260213131000_AddSchoolIdToEnrollments")]
public partial class AddSchoolIdToEnrollments : Migration
{
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
