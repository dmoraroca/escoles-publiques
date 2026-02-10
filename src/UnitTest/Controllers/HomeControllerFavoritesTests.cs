using Xunit;
using Moq;
using Web.Controllers;
using Microsoft.Extensions.Logging;

namespace UnitTest.Controllers
{
    public class HomeControllerFavoritesTests
    {
        [Fact]
        public void GetFakeFavoriteSchools_ReturnsList()
        {
            var logger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(logger.Object);

            var method = typeof(HomeController).GetMethod("GetFakeFavoriteSchools", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var result = method?.Invoke(controller, new object[] { "1" }) as System.Collections.IEnumerable;

            Assert.NotNull(result);
        }
    }
}
