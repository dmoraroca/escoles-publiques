using Xunit;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.Infrastructure
{
    public class EnrollmentRepositoryTests
    {
        private SchoolDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseInMemoryDatabase(databaseName: "EnrollmentDbTest")
                .Options;
            return new SchoolDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEnrollments()
        {
            using var context = GetInMemoryDbContext();
            // Afegir escola i estudiants relacionats
            context.Schools.Add(new School { Id = 1, Name = "Test School", Code = "TST1", CreatedAt = System.DateTime.UtcNow });
            context.Students.AddRange(new List<Student>
            {
                new Student { Id = 1, SchoolId = 1, CreatedAt = System.DateTime.UtcNow },
                new Student { Id = 2, SchoolId = 1, CreatedAt = System.DateTime.UtcNow }
            });
            context.SaveChanges();
            context.Enrollments.AddRange(new List<Enrollment>
            {
                new Enrollment { Id = 1, AcademicYear = "2025", Status = "Active", EnrolledAt = System.DateTime.UtcNow, StudentId = 1, SchoolId = 1 },
                new Enrollment { Id = 2, AcademicYear = "2026", Status = "Active", EnrolledAt = System.DateTime.UtcNow, StudentId = 2, SchoolId = 1 }
            });
            context.SaveChanges();
            var repo = new EnrollmentRepository(context);
            var result = await repo.GetAllAsync();
            Assert.Equal(2, result.Count());
        }
    }
}
