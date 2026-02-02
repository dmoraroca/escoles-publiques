using Xunit;
using Moq;
using Application.UseCases.Services;
using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace UnitTest.Services
{
    public class StudentServiceTests
    {
        [Fact]
        public async Task GetAllStudentsAsync_ReturnsStudents()
        {
            var repoMock = new Mock<IStudentRepository>();
            var schoolRepoMock = new Mock<ISchoolRepository>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentService>>();
            var students = new List<Student> {
                new Student { Id = 1, SchoolId = 1, CreatedAt = System.DateTime.UtcNow },
                new Student { Id = 2, SchoolId = 1, CreatedAt = System.DateTime.UtcNow }
            };
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(students);
            var service = new StudentService(repoMock.Object, schoolRepoMock.Object, userServiceMock.Object, loggerMock.Object);

            var result = await service.GetAllStudentsAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetStudentByIdAsync_ReturnsStudent_WhenExists()
        {
            var repoMock = new Mock<IStudentRepository>();
            var schoolRepoMock = new Mock<ISchoolRepository>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentService>>();
            var student = new Student { Id = 1, SchoolId = 1, CreatedAt = System.DateTime.UtcNow };
            repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(student);
            var service = new StudentService(repoMock.Object, schoolRepoMock.Object, userServiceMock.Object, loggerMock.Object);

            var result = await service.GetStudentByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetStudentByIdAsync_ReturnsNull_WhenNotExists()
        {
            var repoMock = new Mock<IStudentRepository>();
            var schoolRepoMock = new Mock<ISchoolRepository>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentService>>();
            repoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Student?)null);
            var service = new StudentService(repoMock.Object, schoolRepoMock.Object, userServiceMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<Domain.DomainExceptions.NotFoundException>(async () =>
            {
                await service.GetStudentByIdAsync(2);
            });
        }
    }
}
