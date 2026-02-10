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
    public class UserRepositoryTestsExtra
    {
        private static SchoolDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SchoolDbContext(options);
        }

        [Fact]
        public async Task GetByEmailAsync_IsCaseInsensitive()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Email = "Test@Email.com", FirstName = "A", LastName = "B" });
            context.SaveChanges();
            var repo = new UserRepository(context);

            var result = await repo.GetByEmailAsync("test@email.com");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task EmailExistsAsync_ReturnsTrue_WhenExists()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 2, Email = "a@b.com", FirstName = "A", LastName = "B" });
            context.SaveChanges();
            var repo = new UserRepository(context);

            var exists = await repo.EmailExistsAsync("A@B.COM");

            Assert.True(exists);
        }

        [Fact]
        public async Task UpdateAsync_SetsUpdatedAt()
        {
            using var context = GetInMemoryDbContext();
            var user = new User { Id = 3, Email = "c@d.com", FirstName = "C", LastName = "D" };
            context.Users.Add(user);
            context.SaveChanges();
            var repo = new UserRepository(context);

            await repo.UpdateAsync(user);

            var updated = await context.Users.FirstAsync();
            Assert.True(updated.UpdatedAt > DateTime.MinValue);
        }

        [Fact]
        public async Task AddAndDelete_WorkAsExpected()
        {
            using var context = GetInMemoryDbContext();
            var repo = new UserRepository(context);
            var user = new User { Id = 4, Email = "x@y.com", FirstName = "X", LastName = "Y" };

            await repo.AddAsync(user);
            Assert.Single(context.Users);

            await repo.DeleteAsync(4);
            Assert.Empty(context.Users);
        }
    }
}
