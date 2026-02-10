using Web.Models;
using Xunit;

namespace UnitTest.Models
{
    public class ErrorViewModelTests
    {
        [Fact]
        public void ShowRequestId_False_WhenNull()
        {
            var model = new ErrorViewModel { RequestId = null };

            Assert.False(model.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_True_WhenSet()
        {
            var model = new ErrorViewModel { RequestId = "abc" };

            Assert.True(model.ShowRequestId);
        }
    }
}
