using Xunit;
using Moq;
using Web.Controllers;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace UnitTest.Controllers
{
    public class UserControllerDashboardTests
    {
        [Fact]
        public async Task Dashboard_ReturnsView_WhenUserAndStudentExist()
        {
            // Arrange
            var logger = new Mock<ILogger<UserController>>();
            var studentService = new Mock<IStudentService>();
            var enrollmentService = new Mock<IEnrollmentService>();
            var annualFeeService = new Mock<IAnnualFeeService>();
            var userRepository = new Mock<IUserRepository>();
            var schoolRepository = new Mock<ISchoolRepository>();

            var user = new User { Id = 1, FirstName = "Test", LastName = "User", Email = "test@a.com", Role = "USER" };
            var student = new Student { Id = 10, UserId = 1, CreatedAt = System.DateTime.UtcNow, User = user };
            var enrollment = new Enrollment { Id = 100, StudentId = 10, SchoolId = 5, AcademicYear = "2025", Status = "Active", EnrolledAt = System.DateTime.UtcNow };
            var school = new School { Id = 5, Name = "Test School", Code = "TST1", CreatedAt = System.DateTime.UtcNow };
            var fee = new AnnualFee { Id = 200, EnrollmentId = 100, Amount = 100, Currency = "EUR", DueDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow), Enrollment = enrollment };

            userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            studentService.Setup(s => s.GetAllStudentsAsync()).ReturnsAsync(new List<Student> { student });
            enrollmentService.Setup(e => e.GetAllEnrollmentsAsync()).ReturnsAsync(new List<Enrollment> { enrollment });
            schoolRepository.Setup(s => s.GetByIdAsync(5)).ReturnsAsync(school);
            annualFeeService.Setup(a => a.GetAllAnnualFeesAsync()).ReturnsAsync(new List<AnnualFee> { fee });

            var controller = new UserController(
                logger.Object,
                studentService.Object,
                enrollmentService.Object,
                annualFeeService.Object,
                userRepository.Object,
                schoolRepository.Object
            );

            // Simular context d'usuari autenticat
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("Role", "USER")
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // Act
            var result = await controller.Dashboard();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            Assert.NotNull(controller.ViewBag.User);
            Assert.NotNull(controller.ViewBag.Student);
            Assert.NotNull(controller.ViewBag.Enrollments);
            Assert.NotNull(controller.ViewBag.Fees);
        }
    }
}
