using Xunit;
using Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace UnitTest.Controllers
{
    public class SearchControllerTests
    {
        [Fact]
        public void Index_ReturnsView()
        {
            var controller = new SearchController();

            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Search_RedirectsToHome()
        {
            var controller = new SearchController();

            var result = controller.Search("q");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }
    }
}
