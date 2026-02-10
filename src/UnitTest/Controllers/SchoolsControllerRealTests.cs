using Xunit;
using Moq;
using Web.Controllers;
using Web.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Entities;
using System.Collections.Generic;
using Domain.DomainExceptions;
using Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace UnitTest.Controllers
{
    public class SchoolsControllerRealTests
    {
        [Fact]
        public async Task Index_ReturnsView_WithSchools()
        {
            var schoolServiceMock = new Mock<ISchoolsApiClient>();
            var hubContextMock = new Mock<IHubContext<SchoolHub>>();
            var scopesApiMock = new Mock<IScopesApiClient>();
            var loggerMock = new Mock<ILogger<SchoolsController>>();

            var schools = new List<School> { new School { Id = 1, Name = "Escola 1", Code = "C1" } };
            schoolServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(schools);
            scopesApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope>());

            var controller = new SchoolsController(schoolServiceMock.Object, hubContextMock.Object, scopesApiMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var action = await controller.Index();
            var result = Assert.IsType<ViewResult>(action);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Details_Redirects_WhenNotFound()
        {
            var schoolServiceMock = new Mock<ISchoolsApiClient>();
            var hubContextMock = new Mock<IHubContext<SchoolHub>>();
            var scopesApiMock = new Mock<IScopesApiClient>();
            var loggerMock = new Mock<ILogger<SchoolsController>>();

            schoolServiceMock.Setup(s => s.GetByIdAsync(99)).ThrowsAsync(new NotFoundException("School", 99));
            scopesApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope>());

            var controller = new SchoolsController(schoolServiceMock.Object, hubContextMock.Object, scopesApiMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var result = await controller.Details(99);

            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
