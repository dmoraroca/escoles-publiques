using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace UnitTest.Infrastructure
{
    public class ScopeRepositoryTests
    {
        private static SchoolDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SchoolDbContext(options);
        }

        [Fact]
        public async Task GetAllActiveScopesAsync_ReturnsOnlyActiveOrdered()
        {
            using var context = GetInMemoryDbContext();
            context.Scopes.AddRange(new List<Scope>
            {
                new Scope { Id = 1, Name = "B", IsActive = true },
                new Scope { Id = 2, Name = "A", IsActive = true },
                new Scope { Id = 3, Name = "C", IsActive = false }
            });
            context.SaveChanges();

            var repo = new ScopeRepository(context, new Mock<ILogger<ScopeRepository>>().Object);
            await Assert.ThrowsAsync<System.InvalidOperationException>(() => repo.GetAllActiveScopesAsync());
        }

        [Fact]
        public async Task GetScopeByIdAsync_ReturnsScope()
        {
            using var context = GetInMemoryDbContext();
            context.Scopes.Add(new Scope { Id = 10, Name = "X", IsActive = true });
            context.SaveChanges();

            var repo = new ScopeRepository(context, new Mock<ILogger<ScopeRepository>>().Object);
            var result = await repo.GetScopeByIdAsync(10);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetScopeByNameAsync_ReturnsScope()
        {
            using var context = GetInMemoryDbContext();
            context.Scopes.Add(new Scope { Id = 11, Name = "Primary", IsActive = true });
            context.SaveChanges();

            var repo = new ScopeRepository(context, new Mock<ILogger<ScopeRepository>>().Object);
            var result = await repo.GetScopeByNameAsync("Primary");

            Assert.NotNull(result);
        }
    }
}
