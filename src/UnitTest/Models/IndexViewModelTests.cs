using Web.Models;
using Xunit;

namespace UnitTest.Models
{
    public class IndexViewModelTests
    {
        [Fact]
        public void Lists_AreInitialized()
        {
            var model = new IndexViewModel();

            Assert.NotNull(model.FavoriteSchools);
            Assert.NotNull(model.Scopes);
        }
    }
}
