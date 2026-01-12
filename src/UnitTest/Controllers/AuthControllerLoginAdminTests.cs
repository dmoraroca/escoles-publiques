using Xunit;
using Moq;
using Web.Controllers;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace UnitTest.Controllers
{
    public class AuthControllerLoginAdminTests
    {
        [Fact]
        public async Task Login_Post_Success_Admin_RedirectsToHomeIndex()
        {
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<AuthController>>();
            var userRepoMock = new Mock<Domain.Interfaces.IUserRepository>();

            authServiceMock.Setup(a => a.AuthenticateAsync("admin@x", "pwd")).ReturnsAsync((true, "1", "ADM"));
            userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(new Domain.Entities.User { Id = 1, FirstName = "A", LastName = "B" });

            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            var services = new ServiceCollection();
            services.AddSingleton<Domain.Interfaces.IUserRepository>(userRepoMock.Object);
            var authProviderMock = new Mock<IAuthenticationService>();
            authProviderMock.Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string?>(), It.IsAny<System.Security.Claims.ClaimsPrincipal>(), It.IsAny<AuthenticationProperties?>())).Returns(Task.CompletedTask);
            services.AddSingleton<IAuthenticationService>(authProviderMock.Object);
            var provider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = provider;
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
            controller.Url = Moq.Mock.Of<Microsoft.AspNetCore.Mvc.IUrlHelper>();

            var result = await controller.Login("admin@x", "pwd");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }
    }
}
