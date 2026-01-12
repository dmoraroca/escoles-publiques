using Xunit;
using Moq;
using Web.Controllers;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UnitTest.Controllers
{
    public class AuthControllerLogoutTests
    {
        [Fact]
        public async Task Logout_Post_RedirectsToLogin()
        {
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            var services = new ServiceCollection();
            var authProviderMock = new Mock<IAuthenticationService>();
            authProviderMock.Setup(a => a.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string?>(), It.IsAny<AuthenticationProperties?>()))
                .Returns(Task.CompletedTask);
            services.AddSingleton<IAuthenticationService>(authProviderMock.Object);
            var provider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = provider;
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.Url = Moq.Mock.Of<Microsoft.AspNetCore.Mvc.IUrlHelper>();

            var result = await controller.Logout();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
        }

        [Fact]
        public void Login_Get_Redirects_WhenAuthenticated_Admin()
        {
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            var claims = new[] { new Claim("Role", "ADM") };
            var identity = new ClaimsIdentity(claims, "Test");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "1"));
            var user = new ClaimsPrincipal(identity) { };

            var httpContext = new DefaultHttpContext { User = user };
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            var result = controller.Login();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }
    }
}
