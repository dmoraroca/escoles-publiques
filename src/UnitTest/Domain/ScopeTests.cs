using Domain.Entities;
using Xunit;

namespace UnitTest.DomainTests
{
    public class ScopeTests
    {
        [Fact]
        public void CanSetProperties()
        {
            var scope = new Scope
            {
                Id = 1,
                Name = "Primary",
                Description = "Desc",
                IsActive = false,
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow
            };

            Assert.Equal("Primary", scope.Name);
            Assert.False(scope.IsActive);
        }
    }
}
