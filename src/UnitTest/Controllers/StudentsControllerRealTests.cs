using Xunit;
using Moq;
using Web.Controllers;
using Web.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Entities;
using System.Collections.Generic;

namespace UnitTest.Controllers
{
    public class StudentsControllerRealTests
    {
        [Fact]
        public async Task Index_ReturnsView_WithStudents()
        {
            var studentsApiMock = new Mock<IStudentsApiClient>();
            var schoolsApiMock = new Mock<ISchoolsApiClient>();
            var loggerMock = new Mock<ILogger<StudentsController>>();

            var students = new List<ApiStudent>
            {
                new ApiStudent(1, 1, "X", "Y", "x@y.com", null, 1, "Escola")
            };
            studentsApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(students);
            schoolsApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<School>());

            var controller = new StudentsController(studentsApiMock.Object, schoolsApiMock.Object, loggerMock.Object);

            var action = await controller.Index();
            var result = Assert.IsType<ViewResult>(action);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Details_Redirects_WhenNotFound()
        {
            var studentsApiMock = new Mock<IStudentsApiClient>();
            var schoolsApiMock = new Mock<ISchoolsApiClient>();
            var loggerMock = new Mock<ILogger<StudentsController>>();

            studentsApiMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ApiStudent?)null);

            var controller = new StudentsController(studentsApiMock.Object, schoolsApiMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var result = await controller.Details(99);

            Assert.IsType<RedirectResult>(result);
        }
    }
}
