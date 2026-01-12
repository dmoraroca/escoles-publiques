using Xunit;
using Moq;
using Web.Controllers;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.DomainExceptions;
using Domain.Entities;
using Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace UnitTest.Controllers
{
    public class SchoolsControllerPostTests
    {
        [Fact]
        public async Task Create_Post_Redirects_OnSuccess()
        {
            var schoolServiceMock = new Mock<ISchoolService>();
            var hubContextMock = new Mock<IHubContext<SchoolHub>>();
            var scopeRepoMock = new Mock<Domain.Interfaces.IScopeRepository>();
            var loggerMock = new Mock<ILogger<SchoolsController>>();

            schoolServiceMock.Setup(s => s.CreateSchoolAsync(It.IsAny<School>())).ReturnsAsync(new School { Id = 1 });
            scopeRepoMock.Setup(s => s.GetAllActiveScopesAsync()).ReturnsAsync(new System.Collections.Generic.List<Domain.Entities.Scope>());

            var controller = new SchoolsController(schoolServiceMock.Object, hubContextMock.Object, scopeRepoMock.Object, loggerMock.Object);
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
            var schoolServiceMock = new Mock<ISchoolService>();
            var hubContextMock = new Mock<IHubContext<SchoolHub>>();
            var scopeRepoMock = new Mock<Domain.Interfaces.IScopeRepository>();
            var loggerMock = new Mock<ILogger<SchoolsController>>();

            schoolServiceMock.Setup(s => s.UpdateSchoolAsync(It.IsAny<School>())).ThrowsAsync(new DuplicateEntityException("School"));
            scopeRepoMock.Setup(s => s.GetAllActiveScopesAsync()).ReturnsAsync(new System.Collections.Generic.List<Domain.Entities.Scope>());

            var controller = new SchoolsController(schoolServiceMock.Object, hubContextMock.Object, scopeRepoMock.Object, loggerMock.Object);
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
