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
    public class EnrollmentServiceTests
    {
        [Fact]
        public async Task GetAllEnrollmentsAsync_ReturnsEnrollments()
        {
            var repoMock = new Mock<IEnrollmentRepository>();
            var studentRepoMock = new Mock<IStudentRepository>();
            var loggerMock = new Mock<ILogger<EnrollmentService>>();
            var enrollments = new List<Enrollment> {
                new Enrollment { Id = 1, AcademicYear = "2025", Status = "Active", EnrolledAt = System.DateTime.UtcNow, StudentId = 1, SchoolId = 1 },
                new Enrollment { Id = 2, AcademicYear = "2026", Status = "Active", EnrolledAt = System.DateTime.UtcNow, StudentId = 2, SchoolId = 1 }
            };
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(enrollments);
            var service = new EnrollmentService(repoMock.Object, studentRepoMock.Object, loggerMock.Object);

            var result = await service.GetAllEnrollmentsAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetEnrollmentByIdAsync_ReturnsEnrollment_WhenExists()
        {
            var repoMock = new Mock<IEnrollmentRepository>();
            var studentRepoMock = new Mock<IStudentRepository>();
            var loggerMock = new Mock<ILogger<EnrollmentService>>();
            var enrollment = new Enrollment { Id = 1, AcademicYear = "2025", Status = "Active", EnrolledAt = System.DateTime.UtcNow, StudentId = 1, SchoolId = 1 };
            repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(enrollment);
            var service = new EnrollmentService(repoMock.Object, studentRepoMock.Object, loggerMock.Object);

            var result = await service.GetEnrollmentByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetEnrollmentByIdAsync_ReturnsNull_WhenNotExists()
        {
            var repoMock = new Mock<IEnrollmentRepository>();
            var studentRepoMock = new Mock<IStudentRepository>();
            var loggerMock = new Mock<ILogger<EnrollmentService>>();
            repoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Enrollment?)null);
            var service = new EnrollmentService(repoMock.Object, studentRepoMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<Domain.DomainExceptions.NotFoundException>(async () =>
            {
                await service.GetEnrollmentByIdAsync(2);
            });
        }
    }
}
