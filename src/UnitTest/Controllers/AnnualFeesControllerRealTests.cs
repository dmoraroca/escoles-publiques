using Xunit;
using Moq;
using Web.Controllers;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Entities;
using System.Collections.Generic;

namespace UnitTest.Controllers
{
    public class AnnualFeesControllerRealTests
    {
        [Fact]
        public async Task Index_ReturnsView_WithAnnualFees()
        {
            var annualFeeServiceMock = new Mock<IAnnualFeeService>();
            var enrollmentServiceMock = new Mock<IEnrollmentService>();
            var studentServiceMock = new Mock<IStudentService>();
            var loggerMock = new Mock<ILogger<AnnualFeesController>>();
            var fees = new List<AnnualFee> { new AnnualFee { Id = 1 }, new AnnualFee { Id = 2 } };
            annualFeeServiceMock.Setup(s => s.GetAllAnnualFeesAsync()).ReturnsAsync(fees);
            enrollmentServiceMock.Setup(s => s.GetAllEnrollmentsAsync()).ReturnsAsync(new List<Enrollment>());
            studentServiceMock.Setup(s => s.GetAllStudentsAsync()).ReturnsAsync(new List<Student>());
            var controller = new AnnualFeesController(annualFeeServiceMock.Object, enrollmentServiceMock.Object, studentServiceMock.Object, loggerMock.Object);

            var action = await controller.Index();
            var result = Assert.IsType<ViewResult>(action);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Create_ReturnsView_WithModel_WhenCalled()
        {
            var annualFeeServiceMock = new Mock<IAnnualFeeService>();
            var enrollmentServiceMock = new Mock<IEnrollmentService>();
            var studentServiceMock = new Mock<IStudentService>();
            var loggerMock = new Mock<ILogger<AnnualFeesController>>();
            enrollmentServiceMock.Setup(s => s.GetAllEnrollmentsAsync()).ReturnsAsync(new List<Enrollment>());
            studentServiceMock.Setup(s => s.GetAllStudentsAsync()).ReturnsAsync(new List<Student>());
            var controller = new AnnualFeesController(annualFeeServiceMock.Object, enrollmentServiceMock.Object, studentServiceMock.Object, loggerMock.Object);

            var action = await controller.Create();
            var result = Assert.IsType<ViewResult>(action);

            Assert.NotNull(result);
        }
    }
}
