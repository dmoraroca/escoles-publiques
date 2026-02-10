using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UnitTest.Infrastructure
{
    public class SchoolRepositoryTestsExtra
    {
        private static SchoolDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SchoolDbContext(options);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsSchool()
        {
            using var context = GetInMemoryDbContext();
            context.Schools.Add(new School { Id = 1, Name = "A", Code = "A1", CreatedAt = DateTime.UtcNow });
            context.SaveChanges();
            var repo = new SchoolRepository(context);

            var result = await repo.GetByIdAsync(1);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task DeleteAsync_RemovesSchool()
        {
            using var context = GetInMemoryDbContext();
            context.Schools.Add(new School { Id = 2, Name = "B", Code = "B1", CreatedAt = DateTime.UtcNow });
            context.SaveChanges();
            var repo = new SchoolRepository(context);

            await repo.DeleteAsync(2);

            Assert.Empty(context.Schools);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesSchool()
        {
            using var context = GetInMemoryDbContext();
            var school = new School { Id = 3, Name = "C", Code = "C1", CreatedAt = DateTime.UtcNow };
            context.Schools.Add(school);
            context.SaveChanges();
            var repo = new SchoolRepository(context);

            school.Name = "C-Updated";
            await repo.UpdateAsync(school);

            var updated = await context.Schools.FirstAsync();
            Assert.Equal("C-Updated", updated.Name);
        }
    }
}
