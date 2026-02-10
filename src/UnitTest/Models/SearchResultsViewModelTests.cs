using Web.Models;
using Xunit;

namespace UnitTest.Models
{
    public class SearchResultsViewModelTests
    {
        [Fact]
        public void HasResults_IsFalse_WhenAllCollectionsEmpty()
        {
            var model = new SearchResultsViewModel();

            Assert.False(model.HasResults);
        }

        [Fact]
        public void HasResults_IsTrue_WhenAnyCollectionHasItems()
        {
            var model = new SearchResultsViewModel();
            model.Schools.Add(new SchoolResultViewModel { Id = 1, Name = "School", Code = "S1" });

            Assert.True(model.HasResults);
        }
    }
}
