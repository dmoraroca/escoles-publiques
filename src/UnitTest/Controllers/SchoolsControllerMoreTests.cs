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
            Mock<IScopesApiClient> scopesMock,
            out Mock<ILogger<SchoolsController>> loggerMock)
        {
            loggerMock = new Mock<ILogger<SchoolsController>>();
            var controller = new SchoolsController(schoolMock.Object, hubMock.Object, scopesMock.Object, loggerMock.Object);
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
            var scopesMock = new Mock<IScopesApiClient>();
            var schools = new List<Domain.Entities.School> { new Domain.Entities.School { Id = 1, Name = "A", Code = "A1" } };
            schoolMock.Setup(s => s.GetAllAsync()).ReturnsAsync(schools);
            scopesMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope>());
            var controller = CreateController(schoolMock, hubMock, scopesMock, out var loggerMock);

            var result = await controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Details_NotFound_RedirectsToIndex()
        {
            var schoolMock = new Mock<ISchoolsApiClient>();
            var hubMock = new Mock<IHubContext<SchoolHub>>();
            var scopesMock = new Mock<IScopesApiClient>();
            schoolMock.Setup(s => s.GetByIdAsync(99)).ThrowsAsync(new Domain.DomainExceptions.NotFoundException("School", 99));
            scopesMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope>());
            var controller = CreateController(schoolMock, hubMock, scopesMock, out var loggerMock);

            var result = await controller.Details(99);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_Get_ReturnsView()
        {
            var schoolMock = new Mock<ISchoolsApiClient>();
            var hubMock = new Mock<IHubContext<SchoolHub>>();
            var scopesMock = new Mock<IScopesApiClient>();
            scopesMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope>());
            var controller = CreateController(schoolMock, hubMock, scopesMock, out var loggerMock);

            var result = await controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_DuplicateCode_RedirectsToIndex()
        {
            var schoolMock = new Mock<ISchoolsApiClient>();
            var hubMock = new Mock<IHubContext<SchoolHub>>();
            var scopesMock = new Mock<IScopesApiClient>();
            schoolMock.Setup(s => s.CreateAsync(It.IsAny<Domain.Entities.School>())).ThrowsAsync(new Domain.DomainExceptions.DuplicateEntityException("School", "code", "X"));
            scopesMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope>());
            var controller = CreateController(schoolMock, hubMock, scopesMock, out var loggerMock);

            var model = new SchoolViewModel { Code = "X", Name = "Name" };

            var result = await controller.Create(model);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Delete_Post_NotFound_RedirectsToIndex()
        {
            var schoolMock = new Mock<ISchoolsApiClient>();
            var hubMock = new Mock<IHubContext<SchoolHub>>();
            var scopesMock = new Mock<IScopesApiClient>();
            schoolMock.Setup(s => s.GetByIdAsync(99)).ThrowsAsync(new Domain.DomainExceptions.NotFoundException("School", 99));
            scopesMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope>());
            var controller = CreateController(schoolMock, hubMock, scopesMock, out var loggerMock);

            var result = await controller.Delete(99);

            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
