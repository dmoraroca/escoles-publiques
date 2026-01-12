using Xunit;
using Moq;
using Web.Controllers;
using Application.Interfaces;
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
            var schoolServiceMock = new Mock<ISchoolService>();
            var hubContextMock = new Mock<IHubContext<SchoolHub>>();
            var scopeRepoMock = new Mock<Domain.Interfaces.IScopeRepository>();
            var loggerMock = new Mock<ILogger<SchoolsController>>();

            var schools = new List<School> { new School { Id = 1, Name = "Escola 1" } };
            schoolServiceMock.Setup(s => s.GetAllSchoolsAsync()).ReturnsAsync(schools);
            scopeRepoMock.Setup(s => s.GetAllActiveScopesAsync()).ReturnsAsync(new List<Domain.Entities.Scope>());

            var controller = new SchoolsController(schoolServiceMock.Object, hubContextMock.Object, scopeRepoMock.Object, loggerMock.Object);
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
            var schoolServiceMock = new Mock<ISchoolService>();
            var hubContextMock = new Mock<IHubContext<SchoolHub>>();
            var scopeRepoMock = new Mock<Domain.Interfaces.IScopeRepository>();
            var loggerMock = new Mock<ILogger<SchoolsController>>();

            schoolServiceMock.Setup(s => s.GetSchoolByIdAsync(99)).ThrowsAsync(new NotFoundException("School", 99));
            scopeRepoMock.Setup(s => s.GetAllActiveScopesAsync()).ReturnsAsync(new List<Domain.Entities.Scope>());

            var controller = new SchoolsController(schoolServiceMock.Object, hubContextMock.Object, scopeRepoMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var result = await controller.Details(99);

            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
