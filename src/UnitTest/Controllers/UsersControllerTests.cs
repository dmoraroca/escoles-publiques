using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Controllers;
using Xunit;

namespace UnitTest.Controllers
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task Index_ReturnsOnlyUsersWithStudent()
        {
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>
            {
                new User { Id = 1, FirstName = "A", LastName = "B", Email = "a@b.com", Student = new Student { Id = 10 } },
                new User { Id = 2, FirstName = "C", LastName = "D", Email = "c@d.com", Student = null }
            });
            var logger = new Mock<ILogger<UsersController>>();
            var controller = new UsersController(repo.Object, logger.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var result = await controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Web.Models.UserViewModel>>(view.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Index_ReturnsEmpty_OnException()
        {
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetAllAsync()).ThrowsAsync(new System.Exception("fail"));
            var logger = new Mock<ILogger<UsersController>>();
            var controller = new UsersController(repo.Object, logger.Object);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var result = await controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Web.Models.UserViewModel>>(view.Model);
            Assert.Empty(model);
        }
    }
}
