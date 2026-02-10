using Xunit;
using Moq;
using Web.Controllers;
using Web.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain.Entities;

namespace UnitTest.Controllers
{
    public class EnrollmentsControllerRealTests
    {
        [Fact]
        public async Task Index_ReturnsView_WithEnrollments()
        {
            var enrollmentsApiMock = new Mock<IEnrollmentsApiClient>();
            var studentsApiMock = new Mock<IStudentsApiClient>();
            var schoolsApiMock = new Mock<ISchoolsApiClient>();
            var loggerMock = new Mock<ILogger<EnrollmentsController>>();

            var enrollments = new List<ApiEnrollment>
            {
                new ApiEnrollment(1, 1, "Student A", "2025", null, "Active", System.DateTime.UtcNow, 1, "School A"),
                new ApiEnrollment(2, 2, "Student B", "2026", null, "Active", System.DateTime.UtcNow, 1, "School A")
            };
            enrollmentsApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(enrollments);
            studentsApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiStudent>());
            schoolsApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<School>());

            var controller = new EnrollmentsController(enrollmentsApiMock.Object, studentsApiMock.Object, schoolsApiMock.Object, loggerMock.Object);

            var action = await controller.Index();
            var result = Assert.IsType<ViewResult>(action);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Details_ReturnsView_WhenIdNotFound()
        {
            var enrollmentsApiMock = new Mock<IEnrollmentsApiClient>();
            var studentsApiMock = new Mock<IStudentsApiClient>();
            var schoolsApiMock = new Mock<ISchoolsApiClient>();
            var loggerMock = new Mock<ILogger<EnrollmentsController>>();

            enrollmentsApiMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ApiEnrollment?)null);
            studentsApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiStudent>());
            schoolsApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<School>());

            var controller = new EnrollmentsController(enrollmentsApiMock.Object, studentsApiMock.Object, schoolsApiMock.Object, loggerMock.Object);

            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var result = await controller.Details(99);

            Assert.IsType<ViewResult>(result);
        }
    }
}
