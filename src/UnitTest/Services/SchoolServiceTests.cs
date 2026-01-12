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
    public class SchoolServiceTests
    {
        [Fact]
        public async Task GetAllSchoolsAsync_ReturnsSchools()
        {
            var repoMock = new Mock<ISchoolRepository>();
            var loggerMock = new Mock<ILogger<SchoolService>>();
            var schools = new List<School> {
                new School { Id = 1, Name = "A", Code = "A001", CreatedAt = System.DateTime.UtcNow },
                new School { Id = 2, Name = "B", Code = "B001", CreatedAt = System.DateTime.UtcNow }
            };
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(schools);
            var service = new SchoolService(repoMock.Object, loggerMock.Object);

            var result = await service.GetAllSchoolsAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetSchoolByIdAsync_ReturnsSchool_WhenExists()
        {
            var repoMock = new Mock<ISchoolRepository>();
            var loggerMock = new Mock<ILogger<SchoolService>>();
            var school = new School { Id = 1, Name = "A", Code = "A001", CreatedAt = System.DateTime.UtcNow };
            repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(school);
            var service = new SchoolService(repoMock.Object, loggerMock.Object);

            var result = await service.GetSchoolByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetSchoolByIdAsync_ReturnsNull_WhenNotExists()
        {
            var repoMock = new Mock<ISchoolRepository>();
            var loggerMock = new Mock<ILogger<SchoolService>>();
            repoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((School?)null);
            var service = new SchoolService(repoMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<Domain.DomainExceptions.NotFoundException>(async () =>
            {
                await service.GetSchoolByIdAsync(2);
            });
        }
    }
}
