using Xunit;

namespace UnitTest.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_ReturnsView_WhenRoleIsNull()
        {
            // Arrange
            var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<Web.Controllers.HomeController>>();
            var controller = new Web.Controllers.HomeController(logger.Object);
            var user = new Moq.Mock<System.Security.Claims.ClaimsPrincipal>();
            user.Setup(u => u.FindFirst(System.Security.Claims.ClaimTypes.Role)).Returns((System.Security.Claims.Claim)null);
            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user.Object }
            };

            // Act
            var result = controller.Index(null, null);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(result);
        }

        [Fact]
        public void Index_Redirects_WhenRoleIsUser()
        {
            var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<Web.Controllers.HomeController>>();
            var controller = new Web.Controllers.HomeController(logger.Object);
            var context = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            context.User = new System.Security.Claims.ClaimsPrincipal(new[] {
                new System.Security.Claims.ClaimsIdentity(new[] {
                    new System.Security.Claims.Claim("Role", "USER")
                })
            });
            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = context };
            var result = controller.Index(null, null);
            Assert.IsType<Microsoft.AspNetCore.Mvc.RedirectToActionResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsView()
        {
            var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<Web.Controllers.HomeController>>();
            var controller = new Web.Controllers.HomeController(logger.Object);
            var result = controller.Privacy();
            Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(result);
        }

        [Fact]
        public void Error_ReturnsViewWithModel()
        {
            var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<Web.Controllers.HomeController>>();
            var controller = new Web.Controllers.HomeController(logger.Object);
            // Simular Activity.Current
            System.Diagnostics.Activity activity = new System.Diagnostics.Activity("TestActivity");
            activity.Start();
            System.Diagnostics.Activity.Current = activity;
            var result = controller.Error();
            activity.Stop();
            var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(result);
            Assert.IsType<Web.Models.ErrorViewModel>(viewResult.Model);
        }
    }
}
