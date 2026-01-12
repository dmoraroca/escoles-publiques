using Xunit;
using Moq;
using Web.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace UnitTest.Controllers
{
    public class HomeControllerRealTests
    {
        [Fact]
        public void Privacy_ReturnsView()
        {
            var logger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(logger.Object);

            var result = controller.Privacy();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ReturnsView_WithModel()
        {
            var logger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(logger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() };

            var result = controller.Error() as ViewResult;

            Assert.NotNull(result);
            Assert.NotNull(result.Model);
        }
    }
}
