using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Web.ViewComponents;
using Xunit;

namespace UnitTest.ViewComponents
{
    public class MainMenuViewComponentTests
    {
        [Fact]
        public void Invoke_SetsCurrentController()
        {
            var component = new MainMenuViewComponent();
            component.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    RouteData = new RouteData()
                }
            };
            component.ViewComponentContext.ViewContext.RouteData.Values["controller"] = "Home";

            var result = component.Invoke();

            Assert.IsType<ViewViewComponentResult>(result);
            Assert.Equal("Home", component.ViewBag.CurrentController);
        }
    }
}
