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
    public class SchoolRepositoryTests
    {
        private SchoolDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseInMemoryDatabase(databaseName: "SchoolDbTest")
                .Options;
            return new SchoolDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSchoolsOrderedByName()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            context.Schools.AddRange(new List<School>
            {
                new School { Id = 1, Name = "B", Code = "B001", CreatedAt = System.DateTime.UtcNow },
                new School { Id = 2, Name = "A", Code = "A001", CreatedAt = System.DateTime.UtcNow }
            });
            context.SaveChanges();
            var repo = new SchoolRepository(context);

            // Act
            var result = await repo.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("A", result.First().Name);
        }
    }
}
