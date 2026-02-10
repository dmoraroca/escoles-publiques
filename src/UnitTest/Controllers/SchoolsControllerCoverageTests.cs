using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.DomainExceptions;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Controllers;
using Web.Hubs;
using Web.Models;
using Web.Services.Api;
using Xunit;

namespace UnitTest.Controllers
{
    public class SchoolsControllerCoverageTests
    {
        private static SchoolsController CreateController(
            Mock<ISchoolsApiClient> schoolsApi,
            Mock<IScopesApiClient> scopesApi,
            Mock<IHubContext<SchoolHub>> hub,
            out Mock<IClientProxy> clientProxy)
        {
            clientProxy = new Mock<IClientProxy>();
            var clients = new Mock<IHubClients>();
            clients.Setup(c => c.All).Returns(clientProxy.Object);
            hub.Setup(h => h.Clients).Returns(clients.Object);

            var logger = new Mock<ILogger<SchoolsController>>();
            var controller = new SchoolsController(schoolsApi.Object, hub.Object, scopesApi.Object, logger.Object);
            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            return controller;
        }

        [Fact]
        public async Task Index_ReturnsView_OnUnauthorized()
        {
            var schoolsApi = new Mock<ISchoolsApiClient>();
            var scopesApi = new Mock<IScopesApiClient>();
            var hub = new Mock<IHubContext<SchoolHub>>();
            schoolsApi.Setup(s => s.GetAllAsync()).ThrowsAsync(new HttpRequestException("unauthorized", null, System.Net.HttpStatusCode.Unauthorized));

            var controller = CreateController(schoolsApi, scopesApi, hub, out _);

            var result = await controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsView_WhenFound()
        {
            var schoolsApi = new Mock<ISchoolsApiClient>();
            var scopesApi = new Mock<IScopesApiClient>();
            var hub = new Mock<IHubContext<SchoolHub>>();
            schoolsApi.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new School { Id = 1, Name = "A", Code = "A1", CreatedAt = System.DateTime.UtcNow });
            scopesApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope> { new ApiScope(1, "Scope") });

            var controller = CreateController(schoolsApi, scopesApi, hub, out _);

            var result = await controller.Details(1);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_ReturnsView_WhenModelStateInvalid()
        {
            var schoolsApi = new Mock<ISchoolsApiClient>();
            var scopesApi = new Mock<IScopesApiClient>();
            var hub = new Mock<IHubContext<SchoolHub>>();
            scopesApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope> { new ApiScope(1, "Scope") });

            var controller = CreateController(schoolsApi, scopesApi, hub, out _);
            controller.ModelState.AddModelError("Name", "Required");
            controller.ModelState.AddModelError("Code", "Required");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());

            var result = await controller.Create(new SchoolViewModel());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_ResolvesScopeByName_WhenScopeIdIsText()
        {
            var schoolsApi = new Mock<ISchoolsApiClient>();
            var scopesApi = new Mock<IScopesApiClient>();
            var hub = new Mock<IHubContext<SchoolHub>>();
            scopesApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope> { new ApiScope(2, "Primary") });
            schoolsApi.Setup(s => s.CreateAsync(It.IsAny<School>())).ReturnsAsync(new School { Id = 1, Name = "A", Code = "A1" });

            var controller = CreateController(schoolsApi, scopesApi, hub, out var proxy);
            var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "ScopeId", "Primary" }
            });
            controller.ControllerContext.HttpContext.Request.Form = form;

            var model = new SchoolViewModel { Code = "A1", Name = "A", City = "City" };

            var result = await controller.Create(model);

            Assert.IsType<RedirectToActionResult>(result);
            proxy.Verify(p => p.SendCoreAsync("SchoolCreated", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_Post_ReturnsUnauthorized_OnAjaxUnauthorized()
        {
            var schoolsApi = new Mock<ISchoolsApiClient>();
            var scopesApi = new Mock<IScopesApiClient>();
            var hub = new Mock<IHubContext<SchoolHub>>();
            schoolsApi.Setup(s => s.CreateAsync(It.IsAny<School>())).ThrowsAsync(new HttpRequestException("unauthorized", null, System.Net.HttpStatusCode.Unauthorized));

            var controller = CreateController(schoolsApi, scopesApi, hub, out _);
            controller.ControllerContext.HttpContext.Request.Headers["X-Requested-With"] = "XMLHttpRequest";
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());

            var model = new SchoolViewModel { Code = "A1", Name = "A", City = "City" };

            var result = await controller.Create(model);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ReturnsView_OnDuplicateCode()
        {
            var schoolsApi = new Mock<ISchoolsApiClient>();
            var scopesApi = new Mock<IScopesApiClient>();
            var hub = new Mock<IHubContext<SchoolHub>>();
            schoolsApi.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new School { Id = 1, Name = "A", Code = "A1" });
            schoolsApi.Setup(s => s.UpdateAsync(It.IsAny<long>(), It.IsAny<School>()))
                .ThrowsAsync(new DuplicateEntityException("School"));
            scopesApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope> { new ApiScope(1, "Scope") });

            var controller = CreateController(schoolsApi, scopesApi, hub, out _);
            var model = new SchoolViewModel { Id = 1, Code = "A1", Name = "A", City = "City" };

            var result = await controller.Edit(model);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsRedirect_OnNotFound()
        {
            var schoolsApi = new Mock<ISchoolsApiClient>();
            var scopesApi = new Mock<IScopesApiClient>();
            var hub = new Mock<IHubContext<SchoolHub>>();
            schoolsApi.Setup(s => s.GetByIdAsync(1)).ThrowsAsync(new NotFoundException("School", 1));

            var controller = CreateController(schoolsApi, scopesApi, hub, out _);

            var result = await controller.Delete(1);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ReturnsRedirect_OnSuccess()
        {
            var schoolsApi = new Mock<ISchoolsApiClient>();
            var scopesApi = new Mock<IScopesApiClient>();
            var hub = new Mock<IHubContext<SchoolHub>>();
            schoolsApi.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new School { Id = 1, Name = "A", Code = "A1" });
            scopesApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope>());
            schoolsApi.Setup(s => s.UpdateAsync(It.IsAny<long>(), It.IsAny<School>())).Returns(Task.CompletedTask);

            var controller = CreateController(schoolsApi, scopesApi, hub, out var proxy);
            var result = await controller.Edit(new SchoolViewModel { Id = 1, Code = "A1", Name = "A", City = "City" });

            Assert.IsType<RedirectToActionResult>(result);
            proxy.Verify(p => p.SendCoreAsync("SchoolUpdated", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsRedirect_OnSuccess()
        {
            var schoolsApi = new Mock<ISchoolsApiClient>();
            var scopesApi = new Mock<IScopesApiClient>();
            var hub = new Mock<IHubContext<SchoolHub>>();
            schoolsApi.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new School { Id = 1, Name = "A", Code = "A1" });
            schoolsApi.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

            var controller = CreateController(schoolsApi, scopesApi, hub, out var proxy);
            var result = await controller.Delete(1);

            Assert.IsType<RedirectToActionResult>(result);
            proxy.Verify(p => p.SendCoreAsync("SchoolDeleted", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
