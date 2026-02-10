using Xunit;
using Moq;
using Web.Controllers;
using Web.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UnitTest.Controllers
{
    public class UserControllerPostTests
    {
        [Fact]
        public async Task Dashboard_ReturnsView_WhenUserHasStudent()
        {
            var studentsApiMock = new Mock<IStudentsApiClient>();
            var enrollmentsApiMock = new Mock<IEnrollmentsApiClient>();
            var annualFeesApiMock = new Mock<IAnnualFeesApiClient>();
            var schoolsApiMock = new Mock<ISchoolsApiClient>();
            var loggerMock = new Mock<ILogger<DashboardController>>();

            studentsApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiStudent>
            {
                new ApiStudent(2, 1, "Test", "User", "test@a.com", null, 1, "Escola")
            });
            enrollmentsApiMock.Setup(e => e.GetAllAsync()).ReturnsAsync(new List<ApiEnrollment>());
            annualFeesApiMock.Setup(a => a.GetAllAsync()).ReturnsAsync(new List<ApiAnnualFee>());

            var controller = new DashboardController(
                loggerMock.Object,
                studentsApiMock.Object,
                enrollmentsApiMock.Object,
                annualFeesApiMock.Object,
                schoolsApiMock.Object);
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
        public async Task Dashboard_Redirects_WhenNoUserClaim()
        {
            var studentsApiMock = new Mock<IStudentsApiClient>();
            var enrollmentsApiMock = new Mock<IEnrollmentsApiClient>();
            var annualFeesApiMock = new Mock<IAnnualFeesApiClient>();
            var schoolsApiMock = new Mock<ISchoolsApiClient>();
            var loggerMock = new Mock<ILogger<DashboardController>>();

            var controller = new DashboardController(
                loggerMock.Object,
                studentsApiMock.Object,
                enrollmentsApiMock.Object,
                annualFeesApiMock.Object,
                schoolsApiMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var result = await controller.Dashboard();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Auth", redirect.ControllerName);
        }
    }
}
