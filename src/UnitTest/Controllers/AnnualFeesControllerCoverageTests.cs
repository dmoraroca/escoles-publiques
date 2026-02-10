using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DomainExceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Controllers;
using Web.Models;
using Web.Services.Api;
using Xunit;

namespace UnitTest.Controllers
{
    public class AnnualFeesControllerCoverageTests
    {
        private static AnnualFeesController CreateController(Mock<IAnnualFeesApiClient> feesApi, Mock<IEnrollmentsApiClient> enrollmentsApi, Mock<IStudentsApiClient> studentsApi)
        {
            var logger = new Mock<ILogger<AnnualFeesController>>();
            var controller = new AnnualFeesController(feesApi.Object, enrollmentsApi.Object, studentsApi.Object, logger.Object);
            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            return controller;
        }

        [Fact]
        public async Task Create_Post_ReturnsOk_OnAjaxSuccess()
        {
            var feesApi = new Mock<IAnnualFeesApiClient>();
            var enrollmentsApi = new Mock<IEnrollmentsApiClient>();
            var studentsApi = new Mock<IStudentsApiClient>();
            feesApi.Setup(f => f.CreateAsync(It.IsAny<ApiAnnualFeeIn>()))
                .ReturnsAsync(new ApiAnnualFee(1, 1, "Info", "Student", "2025", null, 100m, "EUR", new DateOnly(2025, 9, 1), null, null, null, null));

            var controller = CreateController(feesApi, enrollmentsApi, studentsApi);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";

            var result = await controller.Create(new AnnualFeeViewModel { EnrollmentId = 1, Amount = 100, Currency = "EUR", DueDate = new DateOnly(2025, 9, 1) });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Create_Post_ReturnsUnauthorized_OnAjaxUnauthorized()
        {
            var feesApi = new Mock<IAnnualFeesApiClient>();
            var enrollmentsApi = new Mock<IEnrollmentsApiClient>();
            var studentsApi = new Mock<IStudentsApiClient>();
            feesApi.Setup(f => f.CreateAsync(It.IsAny<ApiAnnualFeeIn>()))
                .ThrowsAsync(new HttpRequestException("unauthorized", null, System.Net.HttpStatusCode.Unauthorized));

            var controller = CreateController(feesApi, enrollmentsApi, studentsApi);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";

            var result = await controller.Create(new AnnualFeeViewModel { EnrollmentId = 1, Amount = 100, Currency = "EUR", DueDate = new DateOnly(2025, 9, 1) });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Edit_Get_ReturnsView_WhenFound()
        {
            var feesApi = new Mock<IAnnualFeesApiClient>();
            var enrollmentsApi = new Mock<IEnrollmentsApiClient>();
            var studentsApi = new Mock<IStudentsApiClient>();
            feesApi.Setup(f => f.GetByIdAsync(1)).ReturnsAsync(new ApiAnnualFee(1, 1, "Info", "Student", "2025", null, 100m, "EUR", new DateOnly(2025, 9, 1), null, null, 1, "School"));
            enrollmentsApi.Setup(e => e.GetAllAsync()).ReturnsAsync(new List<ApiEnrollment>());

            var controller = CreateController(feesApi, enrollmentsApi, studentsApi);

            var result = await controller.Edit(1);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ReturnsView_WhenModelInvalid()
        {
            var feesApi = new Mock<IAnnualFeesApiClient>();
            var enrollmentsApi = new Mock<IEnrollmentsApiClient>();
            var studentsApi = new Mock<IStudentsApiClient>();
            enrollmentsApi.Setup(e => e.GetAllAsync()).ReturnsAsync(new List<ApiEnrollment>());

            var controller = CreateController(feesApi, enrollmentsApi, studentsApi);
            controller.ModelState.AddModelError("Amount", "Required");

            var result = await controller.Edit(new AnnualFeeViewModel { Id = 1 });

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task FixAmounts_UpdatesLargeFees()
        {
            var feesApi = new Mock<IAnnualFeesApiClient>();
            var enrollmentsApi = new Mock<IEnrollmentsApiClient>();
            var studentsApi = new Mock<IStudentsApiClient>();
            feesApi.Setup(f => f.GetAllAsync()).ReturnsAsync(new List<ApiAnnualFee>
            {
                new ApiAnnualFee(1, 1, "Info", "Student", "2025", null, 20000m, "EUR", new DateOnly(2025, 9, 1), null, null, null, null),
                new ApiAnnualFee(2, 1, "Info", "Student", "2025", null, 100m, "EUR", new DateOnly(2025, 9, 1), null, null, null, null)
            });

            var controller = CreateController(feesApi, enrollmentsApi, studentsApi);

            var result = await controller.FixAmounts();

            Assert.IsType<RedirectToActionResult>(result);
            feesApi.Verify(f => f.UpdateAsync(It.IsAny<long>(), It.IsAny<ApiAnnualFeeIn>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsRedirect_OnNotFound()
        {
            var feesApi = new Mock<IAnnualFeesApiClient>();
            var enrollmentsApi = new Mock<IEnrollmentsApiClient>();
            var studentsApi = new Mock<IStudentsApiClient>();
            feesApi.Setup(f => f.DeleteAsync(1)).ThrowsAsync(new NotFoundException("AnnualFee", 1));

            var controller = CreateController(feesApi, enrollmentsApi, studentsApi);

            var result = await controller.Delete(1);

            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
