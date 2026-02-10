using Xunit;
using Moq;
using Web.Controllers;
using Web.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Web.Models;
using Domain.DomainExceptions;
using Domain.Entities;

namespace UnitTest.Controllers
{
    public class EnrollmentsControllerMoreTests
    {
        private EnrollmentsController CreateController(
            Mock<IEnrollmentsApiClient> enrollmentMock,
            Mock<IStudentsApiClient> studentMock,
            Mock<ISchoolsApiClient> schoolMock,
            out Mock<ILogger<EnrollmentsController>> loggerMock)
        {
            loggerMock = new Mock<ILogger<EnrollmentsController>>();
            var controller = new EnrollmentsController(enrollmentMock.Object, studentMock.Object, schoolMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
            return controller;
        }

        [Fact]
        public async Task Create_Post_InvalidModel_RedirectsToIndex()
        {
            var enrollmentMock = new Mock<IEnrollmentsApiClient>();
            var studentMock = new Mock<IStudentsApiClient>();
            var schoolMock = new Mock<ISchoolsApiClient>();
            var controller = CreateController(enrollmentMock, studentMock, schoolMock, out var loggerMock);

            controller.ModelState.AddModelError("StudentId", "Required");

            var result = await controller.Create(new EnrollmentViewModel());

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_Post_SchoolIdZero_RedirectsToIndex()
        {
            var enrollmentMock = new Mock<IEnrollmentsApiClient>();
            var studentMock = new Mock<IStudentsApiClient>();
            var schoolMock = new Mock<ISchoolsApiClient>();
            var controller = CreateController(enrollmentMock, studentMock, schoolMock, out var loggerMock);

            var model = new EnrollmentViewModel { SchoolId = 0 };

            var result = await controller.Create(model);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_Post_Success_CallsApi_AndRedirects()
        {
            var enrollmentMock = new Mock<IEnrollmentsApiClient>();
            var studentMock = new Mock<IStudentsApiClient>();
            var schoolMock = new Mock<ISchoolsApiClient>();
            enrollmentMock.Setup(s => s.CreateAsync(It.IsAny<ApiEnrollmentIn>()))
                .ReturnsAsync(new ApiEnrollment(1, 2, "Student", "2025", null, "Active", System.DateTime.UtcNow, 1, "School"))
                .Verifiable();
            var controller = CreateController(enrollmentMock, studentMock, schoolMock, out var loggerMock);

            var model = new EnrollmentViewModel { SchoolId = 1, StudentId = 2, AcademicYear = "2025", Status = "Active" };

            var result = await controller.Create(model);

            enrollmentMock.Verify();
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_Post_NotFoundException_RedirectsToIndex()
        {
            var enrollmentMock = new Mock<IEnrollmentsApiClient>();
            var studentMock = new Mock<IStudentsApiClient>();
            var schoolMock = new Mock<ISchoolsApiClient>();
            enrollmentMock.Setup(s => s.CreateAsync(It.IsAny<ApiEnrollmentIn>()))
                .ThrowsAsync(new NotFoundException("Student", 5));
            var controller = CreateController(enrollmentMock, studentMock, schoolMock, out var loggerMock);

            var model = new EnrollmentViewModel { SchoolId = 1, StudentId = 5, AcademicYear = "2025", Status = "Active" };

            var result = await controller.Create(model);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsView()
        {
            var enrollmentMock = new Mock<IEnrollmentsApiClient>();
            var studentMock = new Mock<IStudentsApiClient>();
            var schoolMock = new Mock<ISchoolsApiClient>();
            studentMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiStudent>());
            schoolMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<School>());
            var controller = CreateController(enrollmentMock, studentMock, schoolMock, out var loggerMock);

            controller.ModelState.AddModelError("CourseName", "Required");
            var model = new EnrollmentViewModel { Id = 1 };

            var result = await controller.Edit(model);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Delete_Post_NotFoundException_RedirectsToIndex()
        {
            var enrollmentMock = new Mock<IEnrollmentsApiClient>();
            var studentMock = new Mock<IStudentsApiClient>();
            var schoolMock = new Mock<ISchoolsApiClient>();
            enrollmentMock.Setup(s => s.DeleteAsync(99)).ThrowsAsync(new NotFoundException("Enrollment", 99));
            var controller = CreateController(enrollmentMock, studentMock, schoolMock, out var loggerMock);

            var result = await controller.Delete(99);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Delete_Post_Success_RedirectsToIndex()
        {
            var enrollmentMock = new Mock<IEnrollmentsApiClient>();
            var studentMock = new Mock<IStudentsApiClient>();
            var schoolMock = new Mock<ISchoolsApiClient>();
            enrollmentMock.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask).Verifiable();
            var controller = CreateController(enrollmentMock, studentMock, schoolMock, out var loggerMock);

            var result = await controller.Delete(1);

            enrollmentMock.Verify();
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
