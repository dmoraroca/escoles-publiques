using System.Collections.Generic;
using System.Threading.Tasks;
using Application.UseCases.Services;
using Domain.DomainExceptions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace UnitTest.Services
{
    public class AnnualFeeServiceTests
    {
        [Fact]
        public async Task GetAnnualFeeByIdAsync_Throws_WhenNotFound()
        {
            var annualRepo = new Mock<IAnnualFeeRepository>();
            var enrollmentRepo = new Mock<IEnrollmentRepository>();
            var logger = new Mock<ILogger<AnnualFeeService>>();
            annualRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((AnnualFee?)null);

            var service = new AnnualFeeService(annualRepo.Object, enrollmentRepo.Object, logger.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => service.GetAnnualFeeByIdAsync(1));
        }

        [Fact]
        public async Task CreateAnnualFeeAsync_Throws_WhenAmountInvalid()
        {
            var annualRepo = new Mock<IAnnualFeeRepository>();
            var enrollmentRepo = new Mock<IEnrollmentRepository>();
            var logger = new Mock<ILogger<AnnualFeeService>>();
            var service = new AnnualFeeService(annualRepo.Object, enrollmentRepo.Object, logger.Object);

            await Assert.ThrowsAsync<ValidationException>(() =>
                service.CreateAnnualFeeAsync(new AnnualFee { Amount = 0, EnrollmentId = 1 }));
        }

        [Fact]
        public async Task CreateAnnualFeeAsync_Throws_WhenEnrollmentMissing()
        {
            var annualRepo = new Mock<IAnnualFeeRepository>();
            var enrollmentRepo = new Mock<IEnrollmentRepository>();
            var logger = new Mock<ILogger<AnnualFeeService>>();
            enrollmentRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Enrollment?)null);
            var service = new AnnualFeeService(annualRepo.Object, enrollmentRepo.Object, logger.Object);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.CreateAnnualFeeAsync(new AnnualFee { Amount = 10, EnrollmentId = 5 }));
        }

        [Fact]
        public async Task UpdateAnnualFeeAsync_Throws_WhenNotFound()
        {
            var annualRepo = new Mock<IAnnualFeeRepository>();
            var enrollmentRepo = new Mock<IEnrollmentRepository>();
            var logger = new Mock<ILogger<AnnualFeeService>>();
            annualRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((AnnualFee?)null);
            var service = new AnnualFeeService(annualRepo.Object, enrollmentRepo.Object, logger.Object);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.UpdateAnnualFeeAsync(new AnnualFee { Id = 1, Amount = 10, EnrollmentId = 1 }));
        }

        [Fact]
        public async Task DeleteAnnualFeeAsync_Throws_WhenNotFound()
        {
            var annualRepo = new Mock<IAnnualFeeRepository>();
            var enrollmentRepo = new Mock<IEnrollmentRepository>();
            var logger = new Mock<ILogger<AnnualFeeService>>();
            annualRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((AnnualFee?)null);
            var service = new AnnualFeeService(annualRepo.Object, enrollmentRepo.Object, logger.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAnnualFeeAsync(1));
        }

        [Fact]
        public async Task GetAllAnnualFeesAsync_ReturnsFees()
        {
            var annualRepo = new Mock<IAnnualFeeRepository>();
            var enrollmentRepo = new Mock<IEnrollmentRepository>();
            var logger = new Mock<ILogger<AnnualFeeService>>();
            annualRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<AnnualFee> { new AnnualFee { Id = 1 } });
            var service = new AnnualFeeService(annualRepo.Object, enrollmentRepo.Object, logger.Object);

            var result = await service.GetAllAnnualFeesAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task CreateAnnualFeeAsync_Creates_WhenValid()
        {
            var annualRepo = new Mock<IAnnualFeeRepository>();
            var enrollmentRepo = new Mock<IEnrollmentRepository>();
            var logger = new Mock<ILogger<AnnualFeeService>>();
            enrollmentRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Enrollment { Id = 2 });
            annualRepo.Setup(r => r.AddAsync(It.IsAny<AnnualFee>())).ReturnsAsync(new AnnualFee { Id = 5, EnrollmentId = 2, Amount = 10 });

            var service = new AnnualFeeService(annualRepo.Object, enrollmentRepo.Object, logger.Object);

            var result = await service.CreateAnnualFeeAsync(new AnnualFee { EnrollmentId = 2, Amount = 10 });

            Assert.Equal(5, result.Id);
        }
    }
}
