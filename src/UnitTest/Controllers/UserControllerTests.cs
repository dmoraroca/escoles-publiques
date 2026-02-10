using Xunit;
using Moq;

namespace UnitTest.Controllers
{
    public class UserControllerTests
    {
        [Fact]
        public void Constructor_InjectsDependencies_NotNull()
        {
            var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<Web.Controllers.DashboardController>>();
            var studentsApi = new Moq.Mock<Web.Services.Api.IStudentsApiClient>();
            var enrollmentsApi = new Moq.Mock<Web.Services.Api.IEnrollmentsApiClient>();
            var annualFeesApi = new Moq.Mock<Web.Services.Api.IAnnualFeesApiClient>();
            var schoolsApi = new Moq.Mock<Web.Services.Api.ISchoolsApiClient>();

            var controller = new Web.Controllers.DashboardController(
                logger.Object,
                studentsApi.Object,
                enrollmentsApi.Object,
                annualFeesApi.Object,
                schoolsApi.Object
            );

            Assert.NotNull(controller);
        }
    }
}
