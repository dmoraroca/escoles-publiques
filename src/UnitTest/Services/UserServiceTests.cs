using Xunit;
using Moq;
using Application.UseCases.Services;
using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace UnitTest.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenExists()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<UserService>>();
            var user = new User { Id = 1, FirstName = "Test", LastName = "User", Email = "test@a.com", PasswordHash = "hash", Role = "USER" };
            userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            var service = new UserService(userRepoMock.Object, authServiceMock.Object, loggerMock.Object);

            // Act
            var result = await service.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenNotExists()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<UserService>>();
            userRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((User?)null);
            var service = new UserService(userRepoMock.Object, authServiceMock.Object, loggerMock.Object);

            var result = await service.GetUserByIdAsync(2);

            Assert.Null(result);
        }
    }
}
