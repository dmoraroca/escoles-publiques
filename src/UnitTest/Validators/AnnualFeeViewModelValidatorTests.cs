using Web.Models;
using Web.Validators;
using Xunit;

namespace UnitTest.Validators
{
    public class AnnualFeeViewModelValidatorTests
    {
        [Fact]
        public void Validate_Fails_WhenRequiredFieldsMissing()
        {
            var validator = new AnnualFeeViewModelValidator();
            var model = new AnnualFeeViewModel();

            var result = validator.Validate(model);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_Passes_WhenModelValid()
        {
            var validator = new AnnualFeeViewModelValidator();
            var model = new AnnualFeeViewModel
            {
                EnrollmentId = 1,
                Amount = 10,
                DueDate = new System.DateOnly(2025, 9, 1)
            };

            var result = validator.Validate(model);

            Assert.True(result.IsValid);
        }
    }
}
