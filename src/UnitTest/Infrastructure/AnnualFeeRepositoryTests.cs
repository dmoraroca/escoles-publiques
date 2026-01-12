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
    public class AnnualFeeRepositoryTests
    {
        private SchoolDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseInMemoryDatabase(databaseName: "AnnualFeeDbTest")
                .Options;
            return new SchoolDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAnnualFees()
        {
            using var context = GetInMemoryDbContext();
            // Afegir escola, estudiants i inscripcions relacionades
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
            context.AnnualFees.AddRange(new List<AnnualFee>
            {
                new AnnualFee { Id = 1, Amount = 100, Currency = "EUR", DueDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow), EnrollmentId = 1 },
                new AnnualFee { Id = 2, Amount = 200, Currency = "EUR", DueDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow), EnrollmentId = 2 }
            });
            context.SaveChanges();
            var repo = new AnnualFeeRepository(context);
            var result = await repo.GetAllAsync();
            Assert.Equal(2, result.Count());
        }
    }
}
