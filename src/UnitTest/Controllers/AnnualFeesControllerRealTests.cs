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
    public class AnnualFeesControllerRealTests
    {
        [Fact]
        public async Task Index_ReturnsView_WithAnnualFees()
        {
            var annualFeesApiMock = new Mock<IAnnualFeesApiClient>();
            var enrollmentsApiMock = new Mock<IEnrollmentsApiClient>();
            var studentsApiMock = new Mock<IStudentsApiClient>();
            var loggerMock = new Mock<ILogger<AnnualFeesController>>();

            var fees = new List<ApiAnnualFee>
            {
                new ApiAnnualFee(1, 1, "Info", "Student", "2025", null, 100m, "EUR", new DateOnly(2025, 9, 1), null, null, null, null),
                new ApiAnnualFee(2, 1, "Info", "Student", "2025", null, 200m, "EUR", new DateOnly(2025, 9, 1), null, null, null, null)
            };
            annualFeesApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(fees);
            enrollmentsApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiEnrollment>());
            studentsApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiStudent>());

            var controller = new AnnualFeesController(annualFeesApiMock.Object, enrollmentsApiMock.Object, studentsApiMock.Object, loggerMock.Object);

            var action = await controller.Index();
            var result = Assert.IsType<ViewResult>(action);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Create_ReturnsView_WithModel_WhenCalled()
        {
            var annualFeesApiMock = new Mock<IAnnualFeesApiClient>();
            var enrollmentsApiMock = new Mock<IEnrollmentsApiClient>();
            var studentsApiMock = new Mock<IStudentsApiClient>();
            var loggerMock = new Mock<ILogger<AnnualFeesController>>();

            enrollmentsApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiEnrollment>());
            studentsApiMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiStudent>());

            var controller = new AnnualFeesController(annualFeesApiMock.Object, enrollmentsApiMock.Object, studentsApiMock.Object, loggerMock.Object);

            var action = await controller.Create();
            var result = Assert.IsType<ViewResult>(action);

            Assert.NotNull(result);
        }
    }
}
