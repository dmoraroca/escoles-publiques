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
    public class UserRepositoryTests
    {
        private SchoolDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseInMemoryDatabase(databaseName: "UserDbTest")
                .Options;
            return new SchoolDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsUsers()
        {
            using var context = GetInMemoryDbContext();
            context.Users.AddRange(new List<User>
            {
                new User { Id = 1, FirstName = "A" },
                new User { Id = 2, FirstName = "B" }
            });
            context.SaveChanges();
            var repo = new UserRepository(context);
            var result = await repo.GetAllAsync();
            Assert.Equal(2, result.Count());
        }
    }
}
