using Xunit;

namespace UnitTest.Controllers
{
    public class UserControllerTests
    {
        [Fact]
        public void Constructor_InjectsDependencies_NotNull()
        {
            // Arrange
            var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<Web.Controllers.UserController>>();
            var studentService = new Moq.Mock<Application.Interfaces.IStudentService>();
            var enrollmentService = new Moq.Mock<Application.Interfaces.IEnrollmentService>();
            var annualFeeService = new Moq.Mock<Application.Interfaces.IAnnualFeeService>();
            var userRepository = new Moq.Mock<Domain.Interfaces.IUserRepository>();
            var schoolRepository = new Moq.Mock<Domain.Interfaces.ISchoolRepository>();

            // Act
            var controller = new Web.Controllers.UserController(
                logger.Object,
                studentService.Object,
                enrollmentService.Object,
                annualFeeService.Object,
                userRepository.Object,
                schoolRepository.Object
            );

            // Assert
            Assert.NotNull(controller);
        }
    }
}
