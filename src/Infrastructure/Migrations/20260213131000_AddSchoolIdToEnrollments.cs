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
        // 1) Add as nullable so we can backfill existing rows safely.
        migrationBuilder.AddColumn<long>(
            name: "school_id",
            table: "enrollments",
            type: "bigint",
            nullable: true);

        // 2) Backfill from the student's school.
        migrationBuilder.Sql(@"
UPDATE enrollments e
SET school_id = s.school_id
FROM students s
WHERE e.student_id = s.id
  AND e.school_id IS NULL;
");

        // 3) Make NOT NULL after backfill.
        migrationBuilder.AlterColumn<long>(
            name: "school_id",
            table: "enrollments",
            type: "bigint",
            nullable: false,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_enrollments_school_id",
            table: "enrollments",
            column: "school_id");

        migrationBuilder.AddForeignKey(
            name: "fk_enrollments_schools_school_id",
            table: "enrollments",
            column: "school_id",
            principalTable: "schools",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_enrollments_schools_school_id",
            table: "enrollments");

        migrationBuilder.DropIndex(
            name: "IX_enrollments_school_id",
            table: "enrollments");

        migrationBuilder.DropColumn(
            name: "school_id",
            table: "enrollments");
    }
}

