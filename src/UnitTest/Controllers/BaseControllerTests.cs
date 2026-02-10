using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Controllers;
using Xunit;

namespace UnitTest.Controllers
{
    public class BaseControllerTests
    {
        private sealed class TestController : BaseController
        {
            public TestController(ILogger logger) : base(logger) { }

            public IActionResult CallHandleError(Exception ex, string action) => HandleError(ex, action);
            public void CallSetSuccessMessage(string message) => SetSuccessMessage(message);
            public void CallSetErrorMessage(string message) => SetErrorMessage(message);
            public bool CallIsUnauthorized(Exception ex) => IsUnauthorized(ex);
            public bool CallIsAjaxRequest() => IsAjaxRequest();
        }

        [Fact]
        public void HandleError_SetsTempDataAndRedirects()
        {
            var logger = new Mock<ILogger>();
            var controller = CreateController(logger.Object);

            var result = controller.CallHandleError(new InvalidOperationException("boom"), "Test");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
            Assert.True(controller.TempData.ContainsKey("Error"));
        }

        [Fact]
        public void SetSuccessMessage_WritesTempData()
        {
            var controller = CreateController(new Mock<ILogger>().Object);

            controller.CallSetSuccessMessage("ok");

            Assert.Equal("ok", controller.TempData["Success"]);
        }

        [Fact]
        public void SetErrorMessage_WritesTempData()
        {
            var controller = CreateController(new Mock<ILogger>().Object);

            controller.CallSetErrorMessage("err");

            Assert.Equal("err", controller.TempData["Error"]);
        }

        [Fact]
        public void IsUnauthorized_ReturnsTrue_ForHttpRequestUnauthorized()
        {
            var controller = CreateController(new Mock<ILogger>().Object);
            var ex = new HttpRequestException("unauthorized", null, System.Net.HttpStatusCode.Unauthorized);

            var result = controller.CallIsUnauthorized(ex);

            Assert.True(result);
        }

        [Fact]
        public void IsAjaxRequest_ReturnsTrue_WhenHeaderPresent()
        {
            var controller = CreateController(new Mock<ILogger>().Object);
            controller.Request.Headers["X-Requested-With"] = "XMLHttpRequest";

            var result = controller.CallIsAjaxRequest();

            Assert.True(result);
        }

        [Fact]
        public void IsAjaxRequest_ReturnsTrue_WhenAcceptsJson()
        {
            var controller = CreateController(new Mock<ILogger>().Object);
            controller.Request.Headers["Accept"] = "application/json";

            var result = controller.CallIsAjaxRequest();

            Assert.True(result);
        }

        private static TestController CreateController(ILogger logger)
        {
            var controller = new TestController(logger);
            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
            return controller;
        }
    }
}
