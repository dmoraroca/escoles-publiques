using Xunit;
using Moq;
using Web.Controllers;
using Web.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Models;
using Domain.DomainExceptions;
using System;

namespace UnitTest.Controllers
{
    public class AnnualFeesControllerPostTests
    {
        [Fact]
        public async Task Create_Post_Redirects_WhenValid()
        {
            var annualFeesApiMock = new Mock<IAnnualFeesApiClient>();
            var enrollmentsApiMock = new Mock<IEnrollmentsApiClient>();
            var studentsApiMock = new Mock<IStudentsApiClient>();
            var loggerMock = new Mock<ILogger<AnnualFeesController>>();

            annualFeesApiMock.Setup(s => s.CreateAsync(It.IsAny<ApiAnnualFeeIn>()))
                .ReturnsAsync(new ApiAnnualFee(1, 1, "Info", "Student", "2025", null, 100m, "EUR", new DateOnly(2025, 9, 1), null, null, null, null));

            var controller = new AnnualFeesController(annualFeesApiMock.Object, enrollmentsApiMock.Object, studentsApiMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var model = new AnnualFeeViewModel { EnrollmentId = 1, Amount = 100, Currency = "EUR", DueDate = DateOnly.FromDateTime(DateTime.UtcNow) };

            var result = await controller.Create(model);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            annualFeesApiMock.Verify(s => s.CreateAsync(It.IsAny<ApiAnnualFeeIn>()), Times.Once);
        }

        [Fact]
        public async Task Create_Post_SetsError_WhenEnrollmentNotFound()
        {
            var annualFeesApiMock = new Mock<IAnnualFeesApiClient>();
            var enrollmentsApiMock = new Mock<IEnrollmentsApiClient>();
            var studentsApiMock = new Mock<IStudentsApiClient>();
            var loggerMock = new Mock<ILogger<AnnualFeesController>>();

            annualFeesApiMock.Setup(s => s.CreateAsync(It.IsAny<ApiAnnualFeeIn>()))
                .ThrowsAsync(new NotFoundException("Enrollment", 1));

            var controller = new AnnualFeesController(annualFeesApiMock.Object, enrollmentsApiMock.Object, studentsApiMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var model = new AnnualFeeViewModel { EnrollmentId = 1, Amount = 100, Currency = "EUR", DueDate = DateOnly.FromDateTime(DateTime.UtcNow) };

            var result = await controller.Create(model);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.True(controller.TempData.ContainsKey("Error"));
        }

        [Fact]
        public async Task Edit_Post_RedirectsToDetails_WhenValid()
        {
            var annualFeesApiMock = new Mock<IAnnualFeesApiClient>();
            var enrollmentsApiMock = new Mock<IEnrollmentsApiClient>();
            var studentsApiMock = new Mock<IStudentsApiClient>();
            var loggerMock = new Mock<ILogger<AnnualFeesController>>();

            annualFeesApiMock.Setup(s => s.UpdateAsync(It.IsAny<long>(), It.IsAny<ApiAnnualFeeIn>()))
                .Returns(Task.CompletedTask);

            var controller = new AnnualFeesController(annualFeesApiMock.Object, enrollmentsApiMock.Object, studentsApiMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var model = new AnnualFeeViewModel { Id = 5, EnrollmentId = 1, Amount = 75, Currency = "EUR", DueDate = DateOnly.FromDateTime(DateTime.UtcNow) };

            var result = await controller.Edit(model);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            annualFeesApiMock.Verify(s => s.UpdateAsync(It.IsAny<long>(), It.IsAny<ApiAnnualFeeIn>()), Times.Once);
        }
    }
}
