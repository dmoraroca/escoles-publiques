using Xunit;
using Moq;
using Web.Controllers;
using Web.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Features;

namespace UnitTest.Controllers
{
    public class AuthControllerLoginAdminTests
    {
        private sealed class TestSessionFeature : ISessionFeature
        {
            public ISession Session { get; set; } = default!;
        }

        private static string CreateJwt(string userId, string role)
        {
            var token = new JwtSecurityToken(
                claims: new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, role)
                });
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static void ConfigureSession(DefaultHttpContext httpContext)
        {
            var sessionMock = new Mock<ISession>();
            sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()));
            httpContext.Features.Set<ISessionFeature>(new TestSessionFeature { Session = sessionMock.Object });
        }

        [Fact]
        public async Task Login_Post_Success_Admin_RedirectsToHomeIndex()
        {
            var authServiceMock = new Mock<IAuthApiClient>();
            var loggerMock = new Mock<ILogger<AuthController>>();

            authServiceMock.Setup(a => a.GetTokenAsync("admin@x", "pwd"))
                .ReturnsAsync(CreateJwt("1", "ADM"));

            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            var services = new ServiceCollection();
            var authProviderMock = new Mock<IAuthenticationService>();
            authProviderMock.Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string?>(), It.IsAny<System.Security.Claims.ClaimsPrincipal>(), It.IsAny<AuthenticationProperties?>())).Returns(Task.CompletedTask);
            services.AddSingleton<IAuthenticationService>(authProviderMock.Object);
            var provider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext();
            ConfigureSession(httpContext);
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
