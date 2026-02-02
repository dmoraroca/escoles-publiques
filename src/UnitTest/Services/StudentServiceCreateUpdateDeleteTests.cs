using Xunit;
using Moq;
using Application.UseCases.Services;
using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Domain.DomainExceptions;
using System.Threading.Tasks;

namespace UnitTest.Services
{
    public class StudentServiceCreateUpdateDeleteTests
    {
        [Fact]
        public async Task CreateStudentAsync_Creates_WhenValid()
        {
            var repoMock = new Mock<IStudentRepository>();
            var schoolRepoMock = new Mock<ISchoolRepository>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentService>>();
            var student = new Student { Id = 0, SchoolId = 10, UserId = 1, User = new User { FirstName = "Nom", LastName = "Cognom" } };
            schoolRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new School { Id = 10 });
            repoMock.Setup(r => r.AddAsync(It.IsAny<Student>())).ReturnsAsync((Student s) => { s.Id = 99; return s; });
            var service = new StudentService(repoMock.Object, schoolRepoMock.Object, userServiceMock.Object, loggerMock.Object);

            var result = await service.CreateStudentAsync(student);

            Assert.NotNull(result);
            Assert.Equal(99, result.Id);
            Assert.Equal(10, result.SchoolId);
        }

        [Fact]
        public async Task CreateStudentAsync_ThrowsValidationException_WhenFirstNameMissing()
        {
            var repoMock = new Mock<IStudentRepository>();
            var schoolRepoMock = new Mock<ISchoolRepository>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentService>>();
            var student = new Student { Id = 0, SchoolId = 10, UserId = 1, User = new User { FirstName = "", LastName = "Cognom" } };
            var service = new StudentService(repoMock.Object, schoolRepoMock.Object, userServiceMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<ValidationException>(() => service.CreateStudentAsync(student));
        }

        [Fact]
        public async Task CreateStudentAsync_ThrowsValidationException_WhenLastNameMissing()
        {
            var repoMock = new Mock<IStudentRepository>();
            var schoolRepoMock = new Mock<ISchoolRepository>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentService>>();
            var student = new Student { Id = 0, SchoolId = 10, UserId = 1, User = new User { FirstName = "Nom", LastName = "" } };
            var service = new StudentService(repoMock.Object, schoolRepoMock.Object, userServiceMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<ValidationException>(() => service.CreateStudentAsync(student));
        }

        [Fact]
        public async Task CreateStudentAsync_ThrowsNotFoundException_WhenSchoolNotExists()
        {
            var repoMock = new Mock<IStudentRepository>();
            var schoolRepoMock = new Mock<ISchoolRepository>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentService>>();
            var student = new Student { Id = 0, SchoolId = 99, UserId = 1, User = new User { FirstName = "Nom", LastName = "Cognom" } };
            schoolRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((School?)null);
            var service = new StudentService(repoMock.Object, schoolRepoMock.Object, userServiceMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => service.CreateStudentAsync(student));
        }

        [Fact]
        public async Task UpdateStudentAsync_Updates_WhenExists()
        {
            var repoMock = new Mock<IStudentRepository>();
            var schoolRepoMock = new Mock<ISchoolRepository>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentService>>();
            var student = new Student { Id = 5, SchoolId = 10 };
            repoMock.Setup(r => r.GetByIdAsync(student.Id)).ReturnsAsync(student);
            repoMock.Setup(r => r.UpdateAsync(student)).Returns(Task.CompletedTask);
            var service = new StudentService(repoMock.Object, schoolRepoMock.Object, userServiceMock.Object, loggerMock.Object);

            await service.UpdateStudentAsync(student);
            repoMock.Verify(r => r.UpdateAsync(student), Times.Once);
        }

        [Fact]
        public async Task UpdateStudentAsync_ThrowsNotFoundException_WhenNotExists()
        {
            var repoMock = new Mock<IStudentRepository>();
            var schoolRepoMock = new Mock<ISchoolRepository>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentService>>();
            var student = new Student { Id = 999, SchoolId = 10 };
            repoMock.Setup(r => r.GetByIdAsync(student.Id)).ReturnsAsync((Student?)null);
            var service = new StudentService(repoMock.Object, schoolRepoMock.Object, userServiceMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateStudentAsync(student));
        }

        [Fact]
        public async Task DeleteStudentAsync_Deletes_WhenExists()
        {
            var repoMock = new Mock<IStudentRepository>();
            var schoolRepoMock = new Mock<ISchoolRepository>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentService>>();
            var student = new Student { Id = 7, SchoolId = 10 };
            repoMock.Setup(r => r.GetByIdAsync(student.Id)).ReturnsAsync(student);
            repoMock.Setup(r => r.DeleteAsync(student.Id)).Returns(Task.CompletedTask);
            var service = new StudentService(repoMock.Object, schoolRepoMock.Object, userServiceMock.Object, loggerMock.Object);

            await service.DeleteStudentAsync(student.Id);
            repoMock.Verify(r => r.DeleteAsync(student.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteStudentAsync_ThrowsNotFoundException_WhenNotExists()
        {
            var repoMock = new Mock<IStudentRepository>();
            var schoolRepoMock = new Mock<ISchoolRepository>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentService>>();
            repoMock.Setup(r => r.GetByIdAsync(888)).ReturnsAsync((Student?)null);
            var service = new StudentService(repoMock.Object, schoolRepoMock.Object, userServiceMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteStudentAsync(888));
        }
    }
}
