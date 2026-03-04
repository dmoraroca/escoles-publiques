using Xunit;
using Moq;
using Application.UseCases.Services;
using Domain.Entities;
using Domain.Interfaces;
using System.Threading.Tasks;

namespace UnitTest.Services
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task AuthenticateAsync_ReturnsSuccess_WhenUserAndPasswordMatch()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var service = new AuthService(userRepoMock.Object);
            var password = "secret";
            var user = new User
            {
                Id = 1,
                Email = "test@a.com",
                PasswordHash = service.HashPassword(password),
                Role = "USER",
                IsActive = true
            };
            userRepoMock.Setup(r => r.GetByEmailAsync("test@a.com")).ReturnsAsync(user);
            var result = await service.AuthenticateAsync("test@a.com", password);

            Assert.True(result.success);
            Assert.Equal("1", result.token);
            Assert.Equal("USER", result.role);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsFail_WhenUserNotFound()
        {
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByEmailAsync("nouser@a.com")).ReturnsAsync((User?)null);
            var service = new AuthService(userRepoMock.Object);

            var result = await service.AuthenticateAsync("nouser@a.com", "any");

            Assert.False(result.success);
            Assert.Null(result.token);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsFail_WhenUserInactive()
        {
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByEmailAsync("inactive@a.com"))
                .ReturnsAsync(new User
                {
                    Id = 3,
                    Email = "inactive@a.com",
                    PasswordHash = "x",
                    IsActive = false
                });
            var service = new AuthService(userRepoMock.Object);

            var result = await service.AuthenticateAsync("inactive@a.com", "any");

            Assert.False(result.success);
            Assert.Null(result.token);
            Assert.Null(result.role);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsFail_WhenPasswordIsWrong()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var service = new AuthService(userRepoMock.Object);
            userRepoMock.Setup(r => r.GetByEmailAsync("test@a.com"))
                .ReturnsAsync(new User
                {
                    Id = 2,
                    Email = "test@a.com",
                    PasswordHash = service.HashPassword("correct"),
                    IsActive = true
                });

            var result = await service.AuthenticateAsync("test@a.com", "wrong");

            Assert.False(result.success);
            Assert.Null(result.token);
        }

        [Fact]
        public void VerifyPassword_ReturnsTrue_WhenHashMatches()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var service = new AuthService(userRepoMock.Object);
            var hash = service.HashPassword("abc123");

            Assert.True(service.VerifyPassword("abc123", hash));
            Assert.False(service.VerifyPassword("other", hash));
        }
    }
}
