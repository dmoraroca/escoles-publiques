using Xunit;
using Moq;
using Web.Controllers;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Entities;
using System.Collections.Generic;
using Web.Models;

namespace UnitTest.Controllers
{
    public class EnrollmentsControllerMoreTests
    {
        private EnrollmentsController CreateController(
            Mock<IEnrollmentService> enrollmentMock,
            Mock<IStudentService> studentMock,
            Mock<ISchoolService> schoolMock,
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
            var enrollmentMock = new Mock<IEnrollmentService>();
            var studentMock = new Mock<IStudentService>();
            var schoolMock = new Mock<ISchoolService>();
            var controller = CreateController(enrollmentMock, studentMock, schoolMock, out var loggerMock);

            controller.ModelState.AddModelError("StudentId", "Required");

            var result = await controller.Create(new EnrollmentViewModel());

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_Post_SchoolIdZero_RedirectsToIndex()
        {
            var enrollmentMock = new Mock<IEnrollmentService>();
            var studentMock = new Mock<IStudentService>();
            var schoolMock = new Mock<ISchoolService>();
            var controller = CreateController(enrollmentMock, studentMock, schoolMock, out var loggerMock);

            var model = new EnrollmentViewModel { SchoolId = 0 };

            var result = await controller.Create(model);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_Post_Success_CallsService_AndRedirects()
        {
            var enrollmentMock = new Mock<IEnrollmentService>();
            var studentMock = new Mock<IStudentService>();
            var schoolMock = new Mock<ISchoolService>();
            enrollmentMock.Setup(s => s.CreateEnrollmentAsync(It.IsAny<Domain.Entities.Enrollment>())).ReturnsAsync(new Domain.Entities.Enrollment { Id = 1 }).Verifiable();
            var controller = CreateController(enrollmentMock, studentMock, schoolMock, out var loggerMock);

            var model = new EnrollmentViewModel { SchoolId = 1, StudentId = 2, AcademicYear = "2025" };

            var result = await controller.Create(model);

            enrollmentMock.Verify();
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_Post_NotFoundException_RedirectsToIndex()
        {
            var enrollmentMock = new Mock<IEnrollmentService>();
            var studentMock = new Mock<IStudentService>();
            var schoolMock = new Mock<ISchoolService>();
            enrollmentMock.Setup(s => s.CreateEnrollmentAsync(It.IsAny<Domain.Entities.Enrollment>())).ThrowsAsync(new Domain.DomainExceptions.NotFoundException("Student", 5));
            var controller = CreateController(enrollmentMock, studentMock, schoolMock, out var loggerMock);

            var model = new EnrollmentViewModel { SchoolId = 1, StudentId = 5 };

            var result = await controller.Create(model);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsView()
        {
            var enrollmentMock = new Mock<IEnrollmentService>();
            var studentMock = new Mock<IStudentService>();
            var schoolMock = new Mock<ISchoolService>();
            var controller = CreateController(enrollmentMock, studentMock, schoolMock, out var loggerMock);

            controller.ModelState.AddModelError("CourseName", "Required");
            var model = new EnrollmentViewModel { Id = 1 };

            var result = await controller.Edit(model);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Delete_Post_NotFoundException_RedirectsToIndex()
        {
            var enrollmentMock = new Mock<IEnrollmentService>();
            var studentMock = new Mock<IStudentService>();
            var schoolMock = new Mock<ISchoolService>();
            enrollmentMock.Setup(s => s.DeleteEnrollmentAsync(99)).ThrowsAsync(new Domain.DomainExceptions.NotFoundException("Enrollment", 99));
            var controller = CreateController(enrollmentMock, studentMock, schoolMock, out var loggerMock);

            var result = await controller.Delete(99);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Delete_Post_Success_RedirectsToIndex()
        {
            var enrollmentMock = new Mock<IEnrollmentService>();
            var studentMock = new Mock<IStudentService>();
            var schoolMock = new Mock<ISchoolService>();
            enrollmentMock.Setup(s => s.DeleteEnrollmentAsync(1)).Returns(Task.CompletedTask).Verifiable();
            var controller = CreateController(enrollmentMock, studentMock, schoolMock, out var loggerMock);

            var result = await controller.Delete(1);

            enrollmentMock.Verify();
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
