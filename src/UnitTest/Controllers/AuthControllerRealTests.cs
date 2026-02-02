using Xunit;
using Moq;
using Web.Controllers;
using Web.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UnitTest.Controllers
{
    public class AuthControllerRealTests
    {
        [Fact]
        public void Login_Get_ReturnsView_WhenNotAuthenticated()
        {
            var authServiceMock = new Mock<IAuthApiClient>();
            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() };

            var result = controller.Login();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_Post_InvalidCredentials_ReturnsViewWithError()
        {
            var authServiceMock = new Mock<IAuthApiClient>();
            authServiceMock.Setup(a => a.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(string.Empty);
            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var result = await controller.Login("", "");

            var view = Assert.IsType<ViewResult>(result);
            Assert.True(controller.TempData.ContainsKey("Error"));
        }
    }
}
