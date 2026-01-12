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
    public class SchoolServiceCreateUpdateDeleteTests
    {
        [Fact]
        public async Task CreateSchoolAsync_Creates_WhenValid()
        {
            var repoMock = new Mock<ISchoolRepository>();
            var loggerMock = new Mock<ILogger<SchoolService>>();
            var school = new School { Id = 0, Code = "A001", Name = "Escola A" };
            repoMock.Setup(r => r.GetByCodeAsync(school.Code)).ReturnsAsync((School?)null);
            repoMock.Setup(r => r.AddAsync(It.IsAny<School>())).ReturnsAsync((School s) => { s.Id = 99; return s; });
            var service = new SchoolService(repoMock.Object, loggerMock.Object);

            var result = await service.CreateSchoolAsync(school);

            Assert.NotNull(result);
            Assert.Equal("A001", result.Code);
            Assert.Equal("Escola A", result.Name);
            Assert.Equal(99, result.Id);
        }

        [Fact]
        public async Task CreateSchoolAsync_ThrowsValidationException_WhenCodeMissing()
        {
            var repoMock = new Mock<ISchoolRepository>();
            var loggerMock = new Mock<ILogger<SchoolService>>();
            var school = new School { Id = 0, Code = "", Name = "Escola A" };
            var service = new SchoolService(repoMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<ValidationException>(() => service.CreateSchoolAsync(school));
        }

        [Fact]
        public async Task CreateSchoolAsync_ThrowsValidationException_WhenNameMissing()
        {
            var repoMock = new Mock<ISchoolRepository>();
            var loggerMock = new Mock<ILogger<SchoolService>>();
            var school = new School { Id = 0, Code = "A001", Name = "" };
            var service = new SchoolService(repoMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<ValidationException>(() => service.CreateSchoolAsync(school));
        }

        [Fact]
        public async Task CreateSchoolAsync_ThrowsDuplicateEntityException_WhenCodeExists()
        {
            var repoMock = new Mock<ISchoolRepository>();
            var loggerMock = new Mock<ILogger<SchoolService>>();
            var school = new School { Id = 0, Code = "A001", Name = "Escola A" };
            repoMock.Setup(r => r.GetByCodeAsync(school.Code)).ReturnsAsync(new School { Id = 1, Code = "A001" });
            var service = new SchoolService(repoMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<DuplicateEntityException>(() => service.CreateSchoolAsync(school));
        }

        [Fact]
        public async Task UpdateSchoolAsync_Updates_WhenExists()
        {
            var repoMock = new Mock<ISchoolRepository>();
            var loggerMock = new Mock<ILogger<SchoolService>>();
            var school = new School { Id = 5, Code = "A001", Name = "Escola A" };
            repoMock.Setup(r => r.GetByIdAsync(school.Id)).ReturnsAsync(school);
            repoMock.Setup(r => r.UpdateAsync(school)).Returns(Task.CompletedTask);
            var service = new SchoolService(repoMock.Object, loggerMock.Object);

            await service.UpdateSchoolAsync(school);
            repoMock.Verify(r => r.UpdateAsync(school), Times.Once);
        }

        [Fact]
        public async Task UpdateSchoolAsync_ThrowsNotFoundException_WhenNotExists()
        {
            var repoMock = new Mock<ISchoolRepository>();
            var loggerMock = new Mock<ILogger<SchoolService>>();
            var school = new School { Id = 999, Code = "A001", Name = "Escola A" };
            repoMock.Setup(r => r.GetByIdAsync(school.Id)).ReturnsAsync((School?)null);
            var service = new SchoolService(repoMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateSchoolAsync(school));
        }

        [Fact]
        public async Task DeleteSchoolAsync_Deletes_WhenExists()
        {
            var repoMock = new Mock<ISchoolRepository>();
            var loggerMock = new Mock<ILogger<SchoolService>>();
            var school = new School { Id = 7, Code = "A001", Name = "Escola A" };
            repoMock.Setup(r => r.GetByIdAsync(school.Id)).ReturnsAsync(school);
            repoMock.Setup(r => r.DeleteAsync(school.Id)).Returns(Task.CompletedTask);
            var service = new SchoolService(repoMock.Object, loggerMock.Object);

            await service.DeleteSchoolAsync(school.Id);
            repoMock.Verify(r => r.DeleteAsync(school.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteSchoolAsync_ThrowsNotFoundException_WhenNotExists()
        {
            var repoMock = new Mock<ISchoolRepository>();
            var loggerMock = new Mock<ILogger<SchoolService>>();
            repoMock.Setup(r => r.GetByIdAsync(888)).ReturnsAsync((School?)null);
            var service = new SchoolService(repoMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteSchoolAsync(888));
        }
    }
}
