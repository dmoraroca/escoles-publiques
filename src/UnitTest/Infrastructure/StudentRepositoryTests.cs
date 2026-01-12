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
    public class StudentRepositoryTests
    {
        private SchoolDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseInMemoryDatabase(databaseName: "StudentDbTest")
                .Options;
            return new SchoolDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsStudents()
        {
            using var context = GetInMemoryDbContext();
            // Afegir escola relacionada
            context.Schools.Add(new School { Id = 1, Name = "Test School", Code = "TST1", CreatedAt = System.DateTime.UtcNow });
            context.SaveChanges();
            context.Students.AddRange(new List<Student>
            {
                new Student { Id = 1, SchoolId = 1, CreatedAt = System.DateTime.UtcNow },
                new Student { Id = 2, SchoolId = 1, CreatedAt = System.DateTime.UtcNow }
            });
            context.SaveChanges();
            var repo = new StudentRepository(context);
            var result = await repo.GetAllAsync();
            Assert.Equal(2, result.Count());
        }
    }
}
