using Xunit;
using Moq;
using Web.Controllers;
using Web.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.DomainExceptions;

namespace UnitTest.Controllers
{
    public class StudentsControllerPostTests
    {
        [Fact]
        public async Task Create_Post_Redirects_WhenValid()
        {
            var studentsApiMock = new Mock<IStudentsApiClient>();
            var schoolsApiMock = new Mock<ISchoolsApiClient>();
            var loggerMock = new Mock<ILogger<StudentsController>>();

            studentsApiMock.Setup(s => s.CreateAsync(It.IsAny<ApiStudentIn>()))
                .ReturnsAsync(new ApiStudent(1, null, "A", "B", "a@b.com", null, 1, "Escola"));

            var controller = new StudentsController(studentsApiMock.Object, schoolsApiMock.Object, loggerMock.Object);
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
            var studentsApiMock = new Mock<IStudentsApiClient>();
            var schoolsApiMock = new Mock<ISchoolsApiClient>();
            var loggerMock = new Mock<ILogger<StudentsController>>();

            studentsApiMock.Setup(s => s.UpdateAsync(It.IsAny<long>(), It.IsAny<ApiStudentIn>()))
                .ThrowsAsync(new NotFoundException("Student", 1));

            var controller = new StudentsController(studentsApiMock.Object, schoolsApiMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var model = new Web.Models.StudentViewModel { Id = 1, FirstName = "A", LastName = "B", Email = "a@b.com", SchoolId = 1 };

            var result = await controller.Edit(model);

            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Students", redirect.Url);
        }
    }
}
