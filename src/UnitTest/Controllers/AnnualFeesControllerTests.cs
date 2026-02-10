using Xunit;

namespace UnitTest.Controllers
{
    public class AnnualFeesControllerTests
    {
        [Fact]
        public async Task Create_ReturnsViewWithEnrollmentsAndStudents()
        {
            var mockAnnualFeesApi = new Moq.Mock<Web.Services.Api.IAnnualFeesApiClient>();
            var mockEnrollmentsApi = new Moq.Mock<Web.Services.Api.IEnrollmentsApiClient>();
            var mockStudentsApi = new Moq.Mock<Web.Services.Api.IStudentsApiClient>();
            var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<Web.Controllers.AnnualFeesController>>();

            mockEnrollmentsApi.Setup(s => s.GetAllAsync()).Returns(System.Threading.Tasks.Task.FromResult<System.Collections.Generic.IEnumerable<Web.Services.Api.ApiEnrollment>>(new System.Collections.Generic.List<Web.Services.Api.ApiEnrollment>()));
            mockStudentsApi.Setup(s => s.GetAllAsync()).Returns(System.Threading.Tasks.Task.FromResult<System.Collections.Generic.IEnumerable<Web.Services.Api.ApiStudent>>(new System.Collections.Generic.List<Web.Services.Api.ApiStudent>()));

            var controller = new Web.Controllers.AnnualFeesController(
                mockAnnualFeesApi.Object,
                mockEnrollmentsApi.Object,
                mockStudentsApi.Object,
                logger.Object
            );

            var result = await controller.Create();
            Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            var mockAnnualFeesApi = new Moq.Mock<Web.Services.Api.IAnnualFeesApiClient>();
            var mockEnrollmentsApi = new Moq.Mock<Web.Services.Api.IEnrollmentsApiClient>();
            var mockStudentsApi = new Moq.Mock<Web.Services.Api.IStudentsApiClient>();
            var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<Web.Controllers.AnnualFeesController>>();

            mockAnnualFeesApi.Setup(s => s.GetAllAsync()).Returns(System.Threading.Tasks.Task.FromResult<System.Collections.Generic.IEnumerable<Web.Services.Api.ApiAnnualFee>>(new System.Collections.Generic.List<Web.Services.Api.ApiAnnualFee>()));
            mockEnrollmentsApi.Setup(s => s.GetAllAsync()).Returns(System.Threading.Tasks.Task.FromResult<System.Collections.Generic.IEnumerable<Web.Services.Api.ApiEnrollment>>(new System.Collections.Generic.List<Web.Services.Api.ApiEnrollment>()));
            mockStudentsApi.Setup(s => s.GetAllAsync()).Returns(System.Threading.Tasks.Task.FromResult<System.Collections.Generic.IEnumerable<Web.Services.Api.ApiStudent>>(new System.Collections.Generic.List<Web.Services.Api.ApiStudent>()));

            var controller = new Web.Controllers.AnnualFeesController(
                mockAnnualFeesApi.Object,
                mockEnrollmentsApi.Object,
                mockStudentsApi.Object,
                logger.Object
            );

            var result = await controller.Index();
            Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(result);
        }
    }
}
