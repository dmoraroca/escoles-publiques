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
    public class UserControllerPostTests
    {
        [Fact]
        public async Task Dashboard_ReturnsView_WhenUserHasStudent()
        {
            var userRepoMock = new Mock<Domain.Interfaces.IUserRepository>();
            var studentServiceMock = new Mock<IStudentService>();
            var annualFeeServiceMock = new Mock<IAnnualFeeService>();
            var enrollmentServiceMock = new Mock<IEnrollmentService>();
            var schoolRepoMock = new Mock<Domain.Interfaces.ISchoolRepository>();
            var loggerMock = new Mock<ILogger<UserController>>();

            var user = new User { Id = 1 };
            userRepoMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(user);
            studentServiceMock.Setup(s => s.GetAllStudentsAsync()).ReturnsAsync(new List<Student> { new Student { Id = 2, UserId = 1, User = user } });

            var controller = new UserController(loggerMock.Object, studentServiceMock.Object, enrollmentServiceMock.Object, annualFeeServiceMock.Object, userRepoMock.Object, schoolRepoMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[] {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "1"),
                new System.Security.Claims.Claim("Role", "USER")
            }, "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var result = await controller.Dashboard();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Dashboard_Redirects_WhenUserNotFound()
        {
            var userRepoMock = new Mock<Domain.Interfaces.IUserRepository>();
            var studentServiceMock = new Mock<IStudentService>();
            var annualFeeServiceMock = new Mock<IAnnualFeeService>();
            var enrollmentServiceMock = new Mock<IEnrollmentService>();
            var schoolRepoMock = new Mock<Domain.Interfaces.ISchoolRepository>();
            var loggerMock = new Mock<ILogger<UserController>>();

            userRepoMock.Setup(s => s.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((User?)null);

            var controller = new UserController(loggerMock.Object, studentServiceMock.Object, enrollmentServiceMock.Object, annualFeeServiceMock.Object, userRepoMock.Object, schoolRepoMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var result = await controller.Dashboard();

            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
