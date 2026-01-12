using Xunit;
using Moq;
using Application.UseCases.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Domain.DomainExceptions;
using System.Threading.Tasks;

namespace UnitTest.Services
{
    public class EnrollmentServiceCreateUpdateDeleteTests
    {
        [Fact]
        public async Task CreateEnrollmentAsync_Creates_WhenValid()
        {
            var repoMock = new Mock<IEnrollmentRepository>();
            var studentRepoMock = new Mock<IStudentRepository>();
            var loggerMock = new Mock<ILogger<EnrollmentService>>();
            var enrollment = new Enrollment { Id = 0, AcademicYear = "2026", StudentId = 10 };
            studentRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Student { Id = 10 });
            repoMock.Setup(r => r.AddAsync(It.IsAny<Enrollment>())).ReturnsAsync((Enrollment e) => { e.Id = 99; return e; });
            var service = new EnrollmentService(repoMock.Object, studentRepoMock.Object, loggerMock.Object);

            var result = await service.CreateEnrollmentAsync(enrollment);

            Assert.NotNull(result);
            Assert.Equal(99, result.Id);
            Assert.Equal("2026", result.AcademicYear);
            Assert.Equal(10, result.StudentId);
        }

        [Fact]
        public async Task CreateEnrollmentAsync_ThrowsValidationException_WhenAcademicYearMissing()
        {
            var repoMock = new Mock<IEnrollmentRepository>();
            var studentRepoMock = new Mock<IStudentRepository>();
            var loggerMock = new Mock<ILogger<EnrollmentService>>();
            var enrollment = new Enrollment { Id = 0, AcademicYear = "", StudentId = 10 };
            var service = new EnrollmentService(repoMock.Object, studentRepoMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<ValidationException>(() => service.CreateEnrollmentAsync(enrollment));
        }

        [Fact]
        public async Task CreateEnrollmentAsync_ThrowsValidationException_WhenStudentIdMissing()
        {
            var repoMock = new Mock<IEnrollmentRepository>();
            var studentRepoMock = new Mock<IStudentRepository>();
            var loggerMock = new Mock<ILogger<EnrollmentService>>();
            var enrollment = new Enrollment { Id = 0, AcademicYear = "2026", StudentId = 0 };
            var service = new EnrollmentService(repoMock.Object, studentRepoMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<ValidationException>(() => service.CreateEnrollmentAsync(enrollment));
        }

        [Fact]
        public async Task CreateEnrollmentAsync_ThrowsNotFoundException_WhenStudentNotExists()
        {
            var repoMock = new Mock<IEnrollmentRepository>();
            var studentRepoMock = new Mock<IStudentRepository>();
            var loggerMock = new Mock<ILogger<EnrollmentService>>();
            var enrollment = new Enrollment { Id = 0, AcademicYear = "2026", StudentId = 99 };
            studentRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Student?)null);
            var service = new EnrollmentService(repoMock.Object, studentRepoMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => service.CreateEnrollmentAsync(enrollment));
        }

        [Fact]
        public async Task UpdateEnrollmentAsync_Updates_WhenExists()
        {
            var repoMock = new Mock<IEnrollmentRepository>();
            var studentRepoMock = new Mock<IStudentRepository>();
            var loggerMock = new Mock<ILogger<EnrollmentService>>();
            var enrollment = new Enrollment { Id = 5, AcademicYear = "2026", StudentId = 10 };
            repoMock.Setup(r => r.GetByIdAsync(enrollment.Id)).ReturnsAsync(enrollment);
            repoMock.Setup(r => r.UpdateAsync(enrollment)).Returns(Task.CompletedTask);
            var service = new EnrollmentService(repoMock.Object, studentRepoMock.Object, loggerMock.Object);

            await service.UpdateEnrollmentAsync(enrollment);
            repoMock.Verify(r => r.UpdateAsync(enrollment), Times.Once);
        }

        [Fact]
        public async Task UpdateEnrollmentAsync_ThrowsNotFoundException_WhenNotExists()
        {
            var repoMock = new Mock<IEnrollmentRepository>();
            var studentRepoMock = new Mock<IStudentRepository>();
            var loggerMock = new Mock<ILogger<EnrollmentService>>();
            var enrollment = new Enrollment { Id = 999, AcademicYear = "2026", StudentId = 10 };
            repoMock.Setup(r => r.GetByIdAsync(enrollment.Id)).ReturnsAsync((Enrollment?)null);
            var service = new EnrollmentService(repoMock.Object, studentRepoMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateEnrollmentAsync(enrollment));
        }

        [Fact]
        public async Task DeleteEnrollmentAsync_Deletes_WhenExists()
        {
            var repoMock = new Mock<IEnrollmentRepository>();
            var studentRepoMock = new Mock<IStudentRepository>();
            var loggerMock = new Mock<ILogger<EnrollmentService>>();
            var enrollment = new Enrollment { Id = 7, AcademicYear = "2026", StudentId = 10 };
            repoMock.Setup(r => r.GetByIdAsync(enrollment.Id)).ReturnsAsync(enrollment);
            repoMock.Setup(r => r.DeleteAsync(enrollment.Id)).Returns(Task.CompletedTask);
            var service = new EnrollmentService(repoMock.Object, studentRepoMock.Object, loggerMock.Object);

            await service.DeleteEnrollmentAsync(enrollment.Id);
            repoMock.Verify(r => r.DeleteAsync(enrollment.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteEnrollmentAsync_ThrowsNotFoundException_WhenNotExists()
        {
            var repoMock = new Mock<IEnrollmentRepository>();
            var studentRepoMock = new Mock<IStudentRepository>();
            var loggerMock = new Mock<ILogger<EnrollmentService>>();
            repoMock.Setup(r => r.GetByIdAsync(888)).ReturnsAsync((Enrollment?)null);
            var service = new EnrollmentService(repoMock.Object, studentRepoMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteEnrollmentAsync(888));
        }
    }
}
