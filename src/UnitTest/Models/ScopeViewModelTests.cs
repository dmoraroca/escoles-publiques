using Xunit;

namespace UnitTest.Models
{
    public class ScopeViewModelTests
    {
        [Fact]
        public void Constructor_InitializesWithDefaults()
        {
            var model = new Web.Models.ScopeViewModel();
            Assert.NotNull(model);
            Assert.Equal(string.Empty, model.Name);
            Assert.Equal(string.Empty, model.Url);
        }

        [Fact]
        public void CanSetProperties()
        {
            var model = new Web.Models.ScopeViewModel
            {
                Name = "Test Name",
                Url = "http://test.url"
            };
            Assert.Equal("Test Name", model.Name);
            Assert.Equal("http://test.url", model.Url);
        }
    }
}
