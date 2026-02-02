using Xunit;
using Moq;
using Web.Controllers;
using Web.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Entities;
using System.Collections.Generic;
using Web.Models;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;

namespace UnitTest.Controllers
{
    public class SchoolsControllerMoreTests
    {
        private SchoolsController CreateController(
            Mock<ISchoolsApiClient> schoolMock,
            Mock<IHubContext<SchoolHub>> hubMock,
            Mock<Domain.Interfaces.IScopeRepository> scopeMock,
            out Mock<ILogger<SchoolsController>> loggerMock)
        {
            loggerMock = new Mock<ILogger<SchoolsController>>();
            var controller = new SchoolsController(schoolMock.Object, hubMock.Object, scopeMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
            controller.Url = Mock.Of<IUrlHelper>();
            return controller;
        }

        [Fact]
        public async Task Index_ReturnsView_WithSchools()
        {
            var schoolMock = new Mock<ISchoolsApiClient>();
            var hubMock = new Mock<IHubContext<SchoolHub>>();
            var scopeMock = new Mock<Domain.Interfaces.IScopeRepository>();
            var schools = new List<Domain.Entities.School> { new Domain.Entities.School { Id = 1, Name = "A" } };
            schoolMock.Setup(s => s.GetAllAsync()).ReturnsAsync(schools);
            scopeMock.Setup(s => s.GetAllActiveScopesAsync()).ReturnsAsync(new List<Domain.Entities.Scope>());
            var controller = CreateController(schoolMock, hubMock, scopeMock, out var loggerMock);

            var result = await controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Details_NotFound_RedirectsToIndex()
        {
            var schoolMock = new Mock<ISchoolsApiClient>();
            var hubMock = new Mock<IHubContext<SchoolHub>>();
            var scopeMock = new Mock<Domain.Interfaces.IScopeRepository>();
            schoolMock.Setup(s => s.GetByIdAsync(99)).ThrowsAsync(new Domain.DomainExceptions.NotFoundException("School", 99));
            var controller = CreateController(schoolMock, hubMock, scopeMock, out var loggerMock);

            var result = await controller.Details(99);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Create_Get_ReturnsView()
        {
            var schoolMock = new Mock<ISchoolsApiClient>();
            var hubMock = new Mock<IHubContext<SchoolHub>>();
            var scopeMock = new Mock<Domain.Interfaces.IScopeRepository>();
            var controller = CreateController(schoolMock, hubMock, scopeMock, out var loggerMock);

            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_DuplicateCode_RedirectsToIndex()
        {
            var schoolMock = new Mock<ISchoolsApiClient>();
            var hubMock = new Mock<IHubContext<SchoolHub>>();
            var scopeMock = new Mock<Domain.Interfaces.IScopeRepository>();
            schoolMock.Setup(s => s.CreateAsync(It.IsAny<Domain.Entities.School>())).ThrowsAsync(new Domain.DomainExceptions.DuplicateEntityException("School", "code", "X"));
            var controller = CreateController(schoolMock, hubMock, scopeMock, out var loggerMock);

            var model = new SchoolViewModel { Code = "X", Name = "Name" };

            var result = await controller.Create(model);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Delete_Post_NotFound_RedirectsToIndex()
        {
            var schoolMock = new Mock<ISchoolsApiClient>();
            var hubMock = new Mock<IHubContext<SchoolHub>>();
            var scopeMock = new Mock<Domain.Interfaces.IScopeRepository>();
            schoolMock.Setup(s => s.GetByIdAsync(99)).ThrowsAsync(new Domain.DomainExceptions.NotFoundException("School", 99));
            var controller = CreateController(schoolMock, hubMock, scopeMock, out var loggerMock);

            var result = await controller.Delete(99);

            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
