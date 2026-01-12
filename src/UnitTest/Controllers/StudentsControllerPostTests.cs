using Xunit;
using Moq;
using Web.Controllers;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.DomainExceptions;
using Domain.Entities;

namespace UnitTest.Controllers
{
    public class StudentsControllerPostTests
    {
        [Fact]
        public async Task Create_Post_Redirects_WhenValid()
        {
            var studentServiceMock = new Mock<IStudentService>();
            var schoolServiceMock = new Mock<ISchoolService>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentsController>>();

            userServiceMock.Setup(s => s.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync((Domain.Entities.User?)null);
            userServiceMock.Setup(s => s.CreateUserAsync(It.IsAny<Domain.Entities.User>(), It.IsAny<string>())).ReturnsAsync(new Domain.Entities.User { Id = 10 });
            studentServiceMock.Setup(s => s.CreateStudentAsync(It.IsAny<Student>())).ReturnsAsync(new Student { Id = 20 });

            var controller = new StudentsController(studentServiceMock.Object, schoolServiceMock.Object, userServiceMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var model = new Web.Models.StudentViewModel { FirstName = "A", LastName = "B", Email = "a@b.com", SchoolId = 1 };

            var result = await controller.Create(model);

            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Students", redirect.Url);
        }

        [Fact]
        public async Task Edit_Post_Redirects_WhenNotFound()
        {
            var studentServiceMock = new Mock<IStudentService>();
            var schoolServiceMock = new Mock<ISchoolService>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<StudentsController>>();

            studentServiceMock.Setup(s => s.GetStudentByIdAsync(It.IsAny<int>())).ThrowsAsync(new Domain.DomainExceptions.NotFoundException("Student", 1));

            var controller = new StudentsController(studentServiceMock.Object, schoolServiceMock.Object, userServiceMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var model = new Web.Models.StudentViewModel { Id = 1 };

            var result = await controller.Edit(model);

            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Students", redirect.Url);
        }
    }
}
