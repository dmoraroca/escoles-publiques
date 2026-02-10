using Xunit;
using Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace UnitTest.Controllers
{
    public class ScopesControllerTests
    {
        [Fact]
        public void Index_ReturnsPartialView()
        {
            var controller = new ScopesController();

            var result = controller.Index();

            var partial = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_Scopes", partial.ViewName);
        }
    }
}
