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
    public class UserServiceCreateUpdateTests
    {
        [Fact]
        public async Task CreateUserAsync_CreatesUser_WhenEmailNotExists()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<UserService>>();
            var user = new User { Id = 0, FirstName = "Nou", LastName = "Usuari", Email = "nou@a.com" };
            userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync((User?)null);
            authServiceMock.Setup(a => a.HashPassword(It.IsAny<string>())).Returns("hash");
            userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => { u.Id = 99; return u; });
            var service = new UserService(userRepoMock.Object, authServiceMock.Object, loggerMock.Object);

            var result = await service.CreateUserAsync(user, "password");

            Assert.NotNull(result);
            Assert.Equal("nou@a.com", result.Email);
            Assert.Equal("hash", result.PasswordHash);
            Assert.Equal("USER", result.Role);
            Assert.True(result.IsActive);
            Assert.Equal(99, result.Id);
        }

        [Fact]
        public async Task CreateUserAsync_ThrowsDuplicateEntityException_WhenEmailExists()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<UserService>>();
            var user = new User { Id = 0, FirstName = "Nou", LastName = "Usuari", Email = "existeix@a.com" };
            userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(new User { Id = 1, Email = user.Email });
            var service = new UserService(userRepoMock.Object, authServiceMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<DuplicateEntityException>(() => service.CreateUserAsync(user, "password"));
        }

        [Fact]
        public async Task UpdateUserAsync_Updates_WhenUserExists()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<UserService>>();
            var user = new User { Id = 5, FirstName = "Update", LastName = "User", Email = "update@a.com" };
            userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            userRepoMock.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);
            var service = new UserService(userRepoMock.Object, authServiceMock.Object, loggerMock.Object);

            await service.UpdateUserAsync(user);
            userRepoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ThrowsNotFoundException_WhenUserNotExists()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<UserService>>();
            var user = new User { Id = 999, FirstName = "No", LastName = "Existeix", Email = "no@a.com" };
            userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync((User?)null);
            var service = new UserService(userRepoMock.Object, authServiceMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateUserAsync(user));
        }
    }
}
