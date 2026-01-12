using Xunit;
using Moq;
using Application.UseCases.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace UnitTest.Services
{
    public class AnnualFeeServiceTests
    {
        [Fact]
        public async Task GetAllAnnualFeesAsync_ReturnsAnnualFees()
        {
            var repoMock = new Mock<IAnnualFeeRepository>();
            var enrollmentRepoMock = new Mock<IEnrollmentRepository>();
            var loggerMock = new Mock<ILogger<AnnualFeeService>>();
            var annualFees = new List<AnnualFee> {
                new AnnualFee { Id = 1, Amount = 100, Currency = "EUR", DueDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow), EnrollmentId = 1 },
                new AnnualFee { Id = 2, Amount = 200, Currency = "EUR", DueDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow), EnrollmentId = 2 }
            };
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(annualFees);
            var service = new AnnualFeeService(repoMock.Object, enrollmentRepoMock.Object, loggerMock.Object);

            var result = await service.GetAllAnnualFeesAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAnnualFeeByIdAsync_ReturnsAnnualFee_WhenExists()
        {
            var repoMock = new Mock<IAnnualFeeRepository>();
            var enrollmentRepoMock = new Mock<IEnrollmentRepository>();
            var loggerMock = new Mock<ILogger<AnnualFeeService>>();
            var annualFee = new AnnualFee { Id = 1, Amount = 100, Currency = "EUR", DueDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow), EnrollmentId = 1 };
            repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(annualFee);
            var service = new AnnualFeeService(repoMock.Object, enrollmentRepoMock.Object, loggerMock.Object);

            var result = await service.GetAnnualFeeByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetAnnualFeeByIdAsync_ReturnsNull_WhenNotExists()
        {
            var repoMock = new Mock<IAnnualFeeRepository>();
            var enrollmentRepoMock = new Mock<IEnrollmentRepository>();
            var loggerMock = new Mock<ILogger<AnnualFeeService>>();
            repoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((AnnualFee?)null);
            var service = new AnnualFeeService(repoMock.Object, enrollmentRepoMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<Domain.DomainExceptions.NotFoundException>(async () =>
            {
                await service.GetAnnualFeeByIdAsync(2);
            });
        }
    }
}
