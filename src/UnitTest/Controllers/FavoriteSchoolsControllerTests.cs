using Xunit;
using Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace UnitTest.Controllers
{
    public class FavoriteSchoolsControllerTests
    {
        [Fact]
        public void Index_ReturnsPartialView()
        {
            var controller = new FavoriteSchoolsController();

            var result = controller.Index();

            var partial = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_FavoriteSchools", partial.ViewName);
        }
    }
}
