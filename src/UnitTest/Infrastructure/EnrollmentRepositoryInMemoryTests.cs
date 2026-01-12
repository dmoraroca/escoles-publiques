using Xunit;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.Repositories;
using Domain.Entities;
using System.Threading.Tasks;

namespace UnitTest.Infrastructure
{
    public class EnrollmentRepositoryInMemoryTests
    {
        private SchoolDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new SchoolDbContext(options);
        }

        [Fact]
        public async Task AddUpdateDeleteWorkflow_WorksAgainstInMemoryDb()
        {
            var dbName = "enroll_repo_test_db" + System.Guid.NewGuid();
            using (var context = CreateContext(dbName))
            {
                // seed school and student
                var school = new School { Name = "S", Code = "C", CreatedAt = System.DateTime.UtcNow };
                context.Schools.Add(school);
                await context.SaveChangesAsync();

                var student = new Student { SchoolId = school.Id, CreatedAt = System.DateTime.UtcNow };
                context.Students.Add(student);
                await context.SaveChangesAsync();

                var repo = new EnrollmentRepository(context);

                var enrollment = new Enrollment
                {
                    StudentId = student.Id,
                    AcademicYear = "2025",
                    Status = "active",
                    EnrolledAt = System.DateTime.UtcNow,
                    SchoolId = school.Id
                };

                var created = await repo.AddAsync(enrollment);
                Assert.True(created.Id != 0);

                var all = await repo.GetAllAsync();
                Assert.Contains(all, e => e.Id == created.Id);

                var fetched = await repo.GetByIdAsync(created.Id);
                Assert.NotNull(fetched);

                // update
                fetched.CourseName = "Math";
                await repo.UpdateAsync(fetched);
                var updated = await repo.GetByIdAsync(created.Id);
                Assert.Equal("Math", updated.CourseName);

                // delete
                await repo.DeleteAsync(created.Id);
                var afterDelete = await repo.GetByIdAsync(created.Id);
                Assert.Null(afterDelete);
            }
        }
    }
}
