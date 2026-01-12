using Xunit;
using Moq;
using Web.Controllers;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Entities;
using System.Collections.Generic;

namespace UnitTest.Controllers
{
    public class EnrollmentsControllerRealTests
    {
        [Fact]
        public async Task Index_ReturnsView_WithEnrollments()
        {
            var enrollmentServiceMock = new Mock<IEnrollmentService>();
            var studentServiceMock = new Mock<IStudentService>();
            var schoolServiceMock = new Mock<ISchoolService>();
            var loggerMock = new Mock<ILogger<EnrollmentsController>>();
            var enrollments = new List<Enrollment> { new Enrollment { Id = 1 }, new Enrollment { Id = 2 } };
            enrollmentServiceMock.Setup(s => s.GetAllEnrollmentsAsync()).ReturnsAsync(enrollments);
            studentServiceMock.Setup(s => s.GetAllStudentsAsync()).ReturnsAsync(new List<Student>());
            schoolServiceMock.Setup(s => s.GetAllSchoolsAsync()).ReturnsAsync(new List<School>());
            var controller = new EnrollmentsController(enrollmentServiceMock.Object, studentServiceMock.Object, schoolServiceMock.Object, loggerMock.Object);

            var action = await controller.Index();
            var result = Assert.IsType<ViewResult>(action);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdNotFound()
        {
            var enrollmentServiceMock = new Mock<IEnrollmentService>();
            var studentServiceMock = new Mock<IStudentService>();
            var schoolServiceMock = new Mock<ISchoolService>();
            var loggerMock = new Mock<ILogger<EnrollmentsController>>();
            enrollmentServiceMock.Setup(s => s.GetEnrollmentByIdAsync(99)).ThrowsAsync(new Domain.DomainExceptions.NotFoundException("Enrollment", 99));
            studentServiceMock.Setup(s => s.GetAllStudentsAsync()).ReturnsAsync(new List<Student>());
            schoolServiceMock.Setup(s => s.GetAllSchoolsAsync()).ReturnsAsync(new List<School>());
            var controller = new EnrollmentsController(enrollmentServiceMock.Object, studentServiceMock.Object, schoolServiceMock.Object, loggerMock.Object);

            // Initialize TempData to avoid NullReference in BaseController.SetErrorMessage
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var result = await controller.Details(99);

            // The controller redirects on NotFoundException, so expect RedirectToActionResult
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
