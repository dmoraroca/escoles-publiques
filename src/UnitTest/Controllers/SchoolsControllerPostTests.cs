using Xunit;
using Moq;
using Web.Controllers;
using Web.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.DomainExceptions;
using Domain.Entities;
using Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;

namespace UnitTest.Controllers
{
    public class SchoolsControllerPostTests
    {
        [Fact]
        public async Task Create_Post_Redirects_OnSuccess()
        {
            var schoolServiceMock = new Mock<ISchoolsApiClient>();
            var hubContextMock = new Mock<IHubContext<SchoolHub>>();
            var scopesApiMock = new Mock<IScopesApiClient>();
            var loggerMock = new Mock<ILogger<SchoolsController>>();

            schoolServiceMock.Setup(s => s.CreateAsync(It.IsAny<School>())).ReturnsAsync(new School { Id = 1 });
            scopesApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope>());

            var controller = new SchoolsController(schoolServiceMock.Object, hubContextMock.Object, scopesApiMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var model = new Web.Models.SchoolViewModel { Code = "C1", Name = "Escola 1" };

            var result = await controller.Create(model);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_Post_ReturnsView_WhenDuplicateCode()
        {
            var schoolServiceMock = new Mock<ISchoolsApiClient>();
            var hubContextMock = new Mock<IHubContext<SchoolHub>>();
            var scopesApiMock = new Mock<IScopesApiClient>();
            var loggerMock = new Mock<ILogger<SchoolsController>>();

            schoolServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(new School { Id = 5, Code = "C1", Name = "Escola X" });
            schoolServiceMock.Setup(s => s.UpdateAsync(It.IsAny<long>(), It.IsAny<School>())).ThrowsAsync(new DuplicateEntityException("School"));
            scopesApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope>());

            var controller = new SchoolsController(schoolServiceMock.Object, hubContextMock.Object, scopesApiMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var model = new Web.Models.SchoolViewModel { Id = 5, Code = "C1", Name = "Escola X" };

            var result = await controller.Edit(model);

            var view = Assert.IsType<ViewResult>(result);
            Assert.NotNull(view.Model);
        }
    }
}
