using Web.Models;
using Web.Validators;
using Xunit;

namespace UnitTest.Validators
{
    public class SchoolViewModelValidatorTests
    {
        [Fact]
        public void Validate_Fails_WhenRequiredFieldsMissing()
        {
            var validator = new SchoolViewModelValidator();
            var model = new SchoolViewModel();

            var result = validator.Validate(model);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_Passes_WhenModelValid()
        {
            var validator = new SchoolViewModelValidator();
            var model = new SchoolViewModel
            {
                Code = "C1",
                Name = "Escola",
                City = "Barcelona"
            };

            var result = validator.Validate(model);

            Assert.True(result.IsValid);
        }
    }
}
