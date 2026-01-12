using Xunit;

namespace UnitTest.Controllers
{
    public class AnnualFeesControllerTests
    {
        [Fact]
        public async Task Create_ReturnsViewWithEnrollmentsAndStudents()
        {
            var mockAnnualFeeService = new Moq.Mock<Application.Interfaces.IAnnualFeeService>();
            var mockEnrollmentService = new Moq.Mock<Application.Interfaces.IEnrollmentService>();
            var mockStudentService = new Moq.Mock<Application.Interfaces.IStudentService>();
            var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<Web.Controllers.AnnualFeesController>>();

            mockEnrollmentService.Setup(s => s.GetAllEnrollmentsAsync()).Returns(System.Threading.Tasks.Task.FromResult<IEnumerable<Domain.Entities.Enrollment>>(new List<Domain.Entities.Enrollment>()));
            mockStudentService.Setup(s => s.GetAllStudentsAsync()).Returns(System.Threading.Tasks.Task.FromResult<IEnumerable<Domain.Entities.Student>>(new List<Domain.Entities.Student>()));

            var controller = new Web.Controllers.AnnualFeesController(
                mockAnnualFeeService.Object,
                mockEnrollmentService.Object,
                mockStudentService.Object,
                logger.Object
            );

            var result = await controller.Create();
            Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            var mockAnnualFeeService = new Moq.Mock<Application.Interfaces.IAnnualFeeService>();
            var mockEnrollmentService = new Moq.Mock<Application.Interfaces.IEnrollmentService>();
            var mockStudentService = new Moq.Mock<Application.Interfaces.IStudentService>();
            var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<Web.Controllers.AnnualFeesController>>();

            mockAnnualFeeService.Setup(s => s.GetAllAnnualFeesAsync()).Returns(System.Threading.Tasks.Task.FromResult<IEnumerable<Domain.Entities.AnnualFee>>(new List<Domain.Entities.AnnualFee>()));

            var controller = new Web.Controllers.AnnualFeesController(
                mockAnnualFeeService.Object,
                mockEnrollmentService.Object,
                mockStudentService.Object,
                logger.Object
            );

            var result = await controller.Index();
            Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(result);
        }
    }
}
