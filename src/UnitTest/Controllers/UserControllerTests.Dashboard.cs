using Xunit;
using Moq;
using Web.Controllers;
using Web.Services.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UnitTest.Controllers
{
    public class UserControllerDashboardTests
    {
        [Fact]
        public async Task Dashboard_ReturnsView_WhenUserAndStudentExist()
        {
            var logger = new Mock<ILogger<DashboardController>>();
            var studentsApi = new Mock<IStudentsApiClient>();
            var enrollmentsApi = new Mock<IEnrollmentsApiClient>();
            var annualFeesApi = new Mock<IAnnualFeesApiClient>();
            var schoolsApi = new Mock<ISchoolsApiClient>();

            var student = new ApiStudent(10, 1, "Test", "User", "test@a.com", null, 1, "Test School");
            var enrollment = new ApiEnrollment(100, 10, "Test User", "2025", "Course", "Active", System.DateTime.UtcNow, 1, "Test School");
            var fee = new ApiAnnualFee(200, 100, "Info", "Test User", "2025", "Course", 100, "EUR", new DateOnly(2025, 9, 1), null, null, 1, "Test School");

            studentsApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiStudent> { student });
            enrollmentsApi.Setup(e => e.GetAllAsync()).ReturnsAsync(new List<ApiEnrollment> { enrollment });
            annualFeesApi.Setup(a => a.GetAllAsync()).ReturnsAsync(new List<ApiAnnualFee> { fee });

            var controller = new DashboardController(
                logger.Object,
                studentsApi.Object,
                enrollmentsApi.Object,
                annualFeesApi.Object,
                schoolsApi.Object
            );

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("Role", "USER")
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            var result = await controller.Dashboard();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            Assert.NotNull(controller.ViewBag.User);
            Assert.NotNull(controller.ViewBag.Student);
            Assert.NotNull(controller.ViewBag.Enrollments);
            Assert.NotNull(controller.ViewBag.Fees);
        }
    }
}
