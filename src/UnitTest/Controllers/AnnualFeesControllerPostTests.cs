using Xunit;
using Moq;
using Web.Controllers;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Models;
using Domain.Entities;
using Domain.DomainExceptions;
using System;

namespace UnitTest.Controllers
{
    public class AnnualFeesControllerPostTests
    {
        [Fact]
        public async Task Create_Post_Redirects_WhenValid()
        {
            var annualFeeServiceMock = new Mock<IAnnualFeeService>();
            var enrollmentServiceMock = new Mock<IEnrollmentService>();
            var studentServiceMock = new Mock<IStudentService>();
            var loggerMock = new Mock<ILogger<AnnualFeesController>>();

            annualFeeServiceMock.Setup(s => s.CreateAnnualFeeAsync(It.IsAny<AnnualFee>())).ReturnsAsync(new AnnualFee { Id = 1 });

            var controller = new AnnualFeesController(annualFeeServiceMock.Object, enrollmentServiceMock.Object, studentServiceMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var model = new AnnualFeeViewModel { EnrollmentId = 1, Amount = 100, Currency = "EUR", DueDate = DateOnly.FromDateTime(DateTime.UtcNow) };

            var result = await controller.Create(model);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            annualFeeServiceMock.Verify(s => s.CreateAnnualFeeAsync(It.IsAny<AnnualFee>()), Times.Once);
        }

        [Fact]
        public async Task Create_Post_SetsError_WhenEnrollmentNotFound()
        {
            var annualFeeServiceMock = new Mock<IAnnualFeeService>();
            var enrollmentServiceMock = new Mock<IEnrollmentService>();
            var studentServiceMock = new Mock<IStudentService>();
            var loggerMock = new Mock<ILogger<AnnualFeesController>>();

            annualFeeServiceMock.Setup(s => s.CreateAnnualFeeAsync(It.IsAny<AnnualFee>())).ThrowsAsync(new NotFoundException("Enrollment", 1));

            var controller = new AnnualFeesController(annualFeeServiceMock.Object, enrollmentServiceMock.Object, studentServiceMock.Object, loggerMock.Object);
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
            var annualFeeServiceMock = new Mock<IAnnualFeeService>();
            var enrollmentServiceMock = new Mock<IEnrollmentService>();
            var studentServiceMock = new Mock<IStudentService>();
            var loggerMock = new Mock<ILogger<AnnualFeesController>>();

            var existing = new AnnualFee { Id = 5, EnrollmentId = 1, Amount = 50 };
            annualFeeServiceMock.Setup(s => s.GetAnnualFeeByIdAsync(5)).ReturnsAsync(existing);
            annualFeeServiceMock.Setup(s => s.UpdateAnnualFeeAsync(It.IsAny<AnnualFee>())).Returns(Task.CompletedTask);

            var controller = new AnnualFeesController(annualFeeServiceMock.Object, enrollmentServiceMock.Object, studentServiceMock.Object, loggerMock.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var model = new AnnualFeeViewModel { Id = 5, EnrollmentId = 1, Amount = 75, Currency = "EUR", DueDate = DateOnly.FromDateTime(DateTime.UtcNow) };

            var result = await controller.Edit(model);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            annualFeeServiceMock.Verify(s => s.UpdateAnnualFeeAsync(It.IsAny<AnnualFee>()), Times.Once);
        }
    }
}
