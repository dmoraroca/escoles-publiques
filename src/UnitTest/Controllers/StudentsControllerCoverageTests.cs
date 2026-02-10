using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DomainExceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Controllers;
using Web.Models;
using Web.Services.Api;
using Xunit;

namespace UnitTest.Controllers
{
    public class StudentsControllerCoverageTests
    {
        private static StudentsController CreateController(Mock<IStudentsApiClient> studentsApi, Mock<ISchoolsApiClient> schoolsApi)
        {
            var logger = new Mock<ILogger<StudentsController>>();
            var controller = new StudentsController(studentsApi.Object, schoolsApi.Object, logger.Object);
            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            return controller;
        }

        [Fact]
        public async Task CheckEmail_ReturnsExists_WhenFound()
        {
            var studentsApi = new Mock<IStudentsApiClient>();
            var schoolsApi = new Mock<ISchoolsApiClient>();
            studentsApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiStudent>
            {
                new ApiStudent(1, null, "A", "B", "a@b.com", null, 1, "School")
            });
            var controller = CreateController(studentsApi, schoolsApi);

            var result = await controller.CheckEmail("a@b.com");

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CheckEmail_ReturnsUnauthorized_OnUnauthorized()
        {
            var studentsApi = new Mock<IStudentsApiClient>();
            var schoolsApi = new Mock<ISchoolsApiClient>();
            studentsApi.Setup(s => s.GetAllAsync())
                .ThrowsAsync(new HttpRequestException("unauthorized", null, System.Net.HttpStatusCode.Unauthorized));
            var controller = CreateController(studentsApi, schoolsApi);

            var result = await controller.CheckEmail("a@b.com");

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Create_Post_ReturnsBadRequest_WhenModelStateInvalid()
        {
            var studentsApi = new Mock<IStudentsApiClient>();
            var schoolsApi = new Mock<ISchoolsApiClient>();
            var controller = CreateController(studentsApi, schoolsApi);
            controller.ModelState.AddModelError("FirstName", "Required");

            var result = await controller.Create(new StudentViewModel());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_Post_ReturnsUnauthorized_OnAjaxUnauthorized()
        {
            var studentsApi = new Mock<IStudentsApiClient>();
            var schoolsApi = new Mock<ISchoolsApiClient>();
            studentsApi.Setup(s => s.CreateAsync(It.IsAny<ApiStudentIn>()))
                .ThrowsAsync(new HttpRequestException("unauthorized", null, System.Net.HttpStatusCode.Unauthorized));
            var controller = CreateController(studentsApi, schoolsApi);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";

            var model = new StudentViewModel { FirstName = "A", LastName = "B", Email = "a@b.com", SchoolId = 1 };
            var result = await controller.Create(model);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Create_Post_ReturnsBadRequest_OnDuplicate()
        {
            var studentsApi = new Mock<IStudentsApiClient>();
            var schoolsApi = new Mock<ISchoolsApiClient>();
            studentsApi.Setup(s => s.CreateAsync(It.IsAny<ApiStudentIn>()))
                .ThrowsAsync(new DuplicateEntityException("Student"));
            var controller = CreateController(studentsApi, schoolsApi);

            var model = new StudentViewModel { FirstName = "A", LastName = "B", Email = "a@b.com", SchoolId = 1 };
            var result = await controller.Create(model);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Edit_Get_ReturnsView_WhenFound()
        {
            var studentsApi = new Mock<IStudentsApiClient>();
            var schoolsApi = new Mock<ISchoolsApiClient>();
            studentsApi.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new ApiStudent(1, null, "A", "B", "a@b.com", null, 1, "School"));
            schoolsApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Domain.Entities.School>());
            var controller = CreateController(studentsApi, schoolsApi);

            var result = await controller.Edit(1);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsRedirect_OnNotFound()
        {
            var studentsApi = new Mock<IStudentsApiClient>();
            var schoolsApi = new Mock<ISchoolsApiClient>();
            studentsApi.Setup(s => s.DeleteAsync(1)).ThrowsAsync(new NotFoundException("Student", 1));
            var controller = CreateController(studentsApi, schoolsApi);

            var result = await controller.Delete(1);

            Assert.IsType<RedirectResult>(result);
        }

        [Fact]
        public async Task Create_Post_Redirects_OnSuccess()
        {
            var studentsApi = new Mock<IStudentsApiClient>();
            var schoolsApi = new Mock<ISchoolsApiClient>();
            studentsApi.Setup(s => s.CreateAsync(It.IsAny<ApiStudentIn>())).ReturnsAsync(new ApiStudent(1, null, "A", "B", "a@b.com", null, 1, "School"));
            var controller = CreateController(studentsApi, schoolsApi);

            var result = await controller.Create(new StudentViewModel { FirstName = "A", LastName = "B", Email = "a@b.com", SchoolId = 1 });

            Assert.IsType<RedirectResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ReturnsView_WhenModelInvalid()
        {
            var studentsApi = new Mock<IStudentsApiClient>();
            var schoolsApi = new Mock<ISchoolsApiClient>();
            schoolsApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Domain.Entities.School>());
            var controller = CreateController(studentsApi, schoolsApi);
            controller.ModelState.AddModelError("FirstName", "Required");

            var result = await controller.Edit(new StudentViewModel { Id = 1 });

            Assert.IsType<ViewResult>(result);
        }
    }
}
