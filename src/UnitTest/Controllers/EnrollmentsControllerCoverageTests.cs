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
    public class EnrollmentsControllerCoverageTests
    {
        private static EnrollmentsController CreateController(Mock<IEnrollmentsApiClient> enrollmentsApi, Mock<IStudentsApiClient> studentsApi, Mock<ISchoolsApiClient> schoolsApi)
        {
            var logger = new Mock<ILogger<EnrollmentsController>>();
            var controller = new EnrollmentsController(enrollmentsApi.Object, studentsApi.Object, schoolsApi.Object, logger.Object);
            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            return controller;
        }

        [Fact]
        public async Task Create_Post_ReturnsOk_OnAjaxSuccess()
        {
            var enrollmentsApi = new Mock<IEnrollmentsApiClient>();
            var studentsApi = new Mock<IStudentsApiClient>();
            var schoolsApi = new Mock<ISchoolsApiClient>();
            enrollmentsApi.Setup(e => e.CreateAsync(It.IsAny<ApiEnrollmentIn>()))
                .ReturnsAsync(new ApiEnrollment(1, 1, "Student", "2025", null, "Active", System.DateTime.UtcNow, 1, "School"));

            var controller = CreateController(enrollmentsApi, studentsApi, schoolsApi);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";

            var result = await controller.Create(new EnrollmentViewModel { SchoolId = 1, StudentId = 1, AcademicYear = "2025", Status = "Active" });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ReturnsRedirect_OnNotFound()
        {
            var enrollmentsApi = new Mock<IEnrollmentsApiClient>();
            var studentsApi = new Mock<IStudentsApiClient>();
            var schoolsApi = new Mock<ISchoolsApiClient>();
            enrollmentsApi.Setup(e => e.UpdateAsync(It.IsAny<long>(), It.IsAny<ApiEnrollmentIn>()))
                .ThrowsAsync(new NotFoundException("Enrollment", 1));

            var controller = CreateController(enrollmentsApi, studentsApi, schoolsApi);

            var result = await controller.Edit(new EnrollmentViewModel { Id = 1, StudentId = 1, AcademicYear = "2025", Status = "Active", SchoolId = 1 });

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsView_WhenFound()
        {
            var enrollmentsApi = new Mock<IEnrollmentsApiClient>();
            var studentsApi = new Mock<IStudentsApiClient>();
            var schoolsApi = new Mock<ISchoolsApiClient>();
            enrollmentsApi.Setup(e => e.GetByIdAsync(1)).ReturnsAsync(new ApiEnrollment(1, 1, "Student", "2025", null, "Active", System.DateTime.UtcNow, 1, "School"));
            studentsApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiStudent>());
            schoolsApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Domain.Entities.School>());

            var controller = CreateController(enrollmentsApi, studentsApi, schoolsApi);

            var result = await controller.Details(1);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_Get_Redirects_WhenNotFound()
        {
            var enrollmentsApi = new Mock<IEnrollmentsApiClient>();
            var studentsApi = new Mock<IStudentsApiClient>();
            var schoolsApi = new Mock<ISchoolsApiClient>();
            enrollmentsApi.Setup(e => e.GetByIdAsync(1)).ReturnsAsync((ApiEnrollment?)null);

            var controller = CreateController(enrollmentsApi, studentsApi, schoolsApi);

            var result = await controller.Edit(1);

            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
