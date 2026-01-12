using Xunit;
using Moq;
using Web.Controllers;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Entities;
using System.Collections.Generic;
using Domain.DomainExceptions;

namespace UnitTest.Controllers
{
    public class StudentsControllerRealTests
    {
        [Fact]
        public async Task Index_ReturnsView_WithStudents()
        {
            var studentServiceMock = new Mock<IStudentService>();
            var schoolServiceMock = new Mock<ISchoolService>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentsController>>();

            var students = new List<Student> { new Student { Id = 1, User = new User { FirstName = "X", LastName = "Y" }, School = new School { Name = "Escola" } } };
            studentServiceMock.Setup(s => s.GetAllStudentsAsync()).ReturnsAsync(students);
            schoolServiceMock.Setup(s => s.GetAllSchoolsAsync()).ReturnsAsync(new List<School>());

            var controller = new StudentsController(studentServiceMock.Object, schoolServiceMock.Object, userServiceMock.Object, loggerMock.Object);

            var action = await controller.Index();
            var result = Assert.IsType<ViewResult>(action);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Details_Redirects_WhenNotFound()
        {
            var studentServiceMock = new Mock<IStudentService>();
            var schoolServiceMock = new Mock<ISchoolService>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentsController>>();

            studentServiceMock.Setup(s => s.GetStudentByIdAsync(99)).ThrowsAsync(new NotFoundException("Student", 99));
            schoolServiceMock.Setup(s => s.GetAllSchoolsAsync()).ReturnsAsync(new List<School>());

            var controller = new StudentsController(studentServiceMock.Object, schoolServiceMock.Object, userServiceMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var result = await controller.Details(99);

            Assert.IsType<RedirectResult>(result);
        }
    }
}
