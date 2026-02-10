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
    public class AnnualFeeRepositoryTestsExtra
    {
        private static SchoolDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SchoolDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SchoolDbContext(options);
        }

        [Fact]
        public async Task GetByEnrollmentIdAsync_ReturnsOrdered()
        {
            using var context = GetInMemoryDbContext();
            context.AnnualFees.AddRange(new List<AnnualFee>
            {
                new AnnualFee { Id = 1, EnrollmentId = 1, Amount = 10m, Currency = "EUR", DueDate = new DateOnly(2025, 9, 2) },
                new AnnualFee { Id = 2, EnrollmentId = 1, Amount = 20m, Currency = "EUR", DueDate = new DateOnly(2025, 9, 1) }
            });
            context.SaveChanges();
            var repo = new AnnualFeeRepository(context);

            var result = await repo.GetByEnrollmentIdAsync(1);

            Assert.Equal(2, result.Count());
            Assert.Equal(2, result.First().Id);
        }

        [Fact]
        public async Task DeleteAsync_RemovesEntity_WhenExists()
        {
            using var context = GetInMemoryDbContext();
            context.AnnualFees.Add(new AnnualFee { Id = 5, EnrollmentId = 1, Amount = 10m, Currency = "EUR", DueDate = new DateOnly(2025, 9, 1) });
            context.SaveChanges();
            var repo = new AnnualFeeRepository(context);

            await repo.DeleteAsync(5);

            Assert.Empty(context.AnnualFees);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesEntity()
        {
            using var context = GetInMemoryDbContext();
            var fee = new AnnualFee { Id = 6, EnrollmentId = 1, Amount = 10m, Currency = "EUR", DueDate = new DateOnly(2025, 9, 1) };
            context.AnnualFees.Add(fee);
            context.SaveChanges();
            var repo = new AnnualFeeRepository(context);

            fee.Amount = 20m;
            await repo.UpdateAsync(fee);

            var updated = await context.AnnualFees.FirstAsync();
            Assert.Equal(20m, updated.Amount);
        }
    }
}
