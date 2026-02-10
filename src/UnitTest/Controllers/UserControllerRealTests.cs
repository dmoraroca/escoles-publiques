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
    public class UserControllerRealTests
    {
        [Fact]
        public async Task Dashboard_ReturnsView_WhenUserHasStudent()
        {
            var logger = new Mock<ILogger<DashboardController>>();
            var studentsApiMock = new Mock<IStudentsApiClient>();
            var enrollmentsApiMock = new Mock<IEnrollmentsApiClient>();
            var annualFeesApiMock = new Mock<IAnnualFeesApiClient>();
            var schoolsApiMock = new Mock<ISchoolsApiClient>();

            studentsApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiStudent>
            {
                new ApiStudent(1, 1, "Test", "User", "test@a.com", null, 1, "Escola")
            });
            enrollmentsApiMock.Setup(e => e.GetAllAsync()).ReturnsAsync(new List<ApiEnrollment>());
            annualFeesApiMock.Setup(a => a.GetAllAsync()).ReturnsAsync(new List<ApiAnnualFee>());

            var controller = new DashboardController(
                logger.Object,
                studentsApiMock.Object,
                enrollmentsApiMock.Object,
                annualFeesApiMock.Object,
                schoolsApiMock.Object);

            var userClaims = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[] {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "1"),
                new System.Security.Claims.Claim("Role", "USER")
            }, "mock"));
            controller.ControllerContext = new ControllerContext { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = userClaims } };

            var result = await controller.Dashboard();

            Assert.IsType<ViewResult>(result);
        }
    }
}
