using Xunit;
using Moq;
using Web.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Entities;
using System.Threading.Tasks;

namespace UnitTest.Controllers
{
    public class UserControllerRealTests
    {
        [Fact]
        public async Task Dashboard_ReturnsView_WhenUserHasStudent()
        {
            var logger = new Mock<ILogger<UserController>>();
            var studentServiceMock = new Mock<IStudentService>();
            var enrollmentServiceMock = new Mock<IEnrollmentService>();
            var annualFeeServiceMock = new Mock<IAnnualFeeService>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var schoolRepositoryMock = new Mock<ISchoolRepository>();

            var user = new User { Id = 1, FirstName = "Test", LastName = "User", Email = "test@a.com", Role = "USER" };
            var student = new Student { Id = 1, UserId = 1, User = user };
            userRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            studentServiceMock.Setup(s => s.GetAllStudentsAsync()).ReturnsAsync(new System.Collections.Generic.List<Student> { student });
            enrollmentServiceMock.Setup(e => e.GetAllEnrollmentsAsync()).ReturnsAsync(new System.Collections.Generic.List<Enrollment>());
            schoolRepositoryMock.Setup(s => s.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((School?)null);
            annualFeeServiceMock.Setup(a => a.GetAllAnnualFeesAsync()).ReturnsAsync(new System.Collections.Generic.List<AnnualFee>());

            var controller = new UserController(
                logger.Object,
                studentServiceMock.Object,
                enrollmentServiceMock.Object,
                annualFeeServiceMock.Object,
                userRepositoryMock.Object,
                schoolRepositoryMock.Object);

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
