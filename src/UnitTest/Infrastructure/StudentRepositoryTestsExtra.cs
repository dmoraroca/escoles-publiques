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
    public class StudentRepositoryTestsExtra
    {
        private static SchoolDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SchoolDbContext(options);
        }

        [Fact]
        public async Task GetBySchoolIdAsync_ReturnsStudents()
        {
            using var context = GetInMemoryDbContext();
            context.Students.AddRange(new List<Student>
            {
                new Student { Id = 1, SchoolId = 10, CreatedAt = DateTime.UtcNow },
                new Student { Id = 2, SchoolId = 10, CreatedAt = DateTime.UtcNow }
            });
            context.SaveChanges();
            var repo = new StudentRepository(context);

            var result = await repo.GetBySchoolIdAsync(10);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task DeleteAsync_RemovesStudent()
        {
            using var context = GetInMemoryDbContext();
            context.Students.Add(new Student { Id = 3, SchoolId = 1, CreatedAt = DateTime.UtcNow });
            context.SaveChanges();
            var repo = new StudentRepository(context);

            await repo.DeleteAsync(3);

            Assert.Empty(context.Students);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsStudent()
        {
            using var context = GetInMemoryDbContext();
            var school = new School
            {
                Id = 1,
                Name = "School A",
                Code = "SCH-A",
                CreatedAt = DateTime.UtcNow
            };
            context.Schools.Add(school);
            var student = new Student { SchoolId = school.Id, CreatedAt = DateTime.UtcNow };
            context.Students.Add(student);
            context.SaveChanges();
            var repo = new StudentRepository(context);

            var result = await repo.GetByIdAsync(student.Id);

            Assert.NotNull(result);
        }
    }
}
