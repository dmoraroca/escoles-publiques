using Domain.DomainExceptions;
using Domain.ValueObjects;

namespace UnitTest.ValueObjects;

public class ValueObjectsTests
{
    [Fact]
    public void SchoolCode_Create_NormalizesAndUppercases()
    {
        var code = SchoolCode.Create("  sc-01 ");

        Assert.Equal("SC-01", code.Value);
    }

    [Fact]
    public void SchoolCode_Create_Throws_WhenInvalid()
    {
        Assert.Throws<ValidationException>(() => SchoolCode.Create("@"));
    }

    [Fact]
    public void EmailAddress_Create_Normalizes()
    {
        var email = EmailAddress.Create("  USER@Example.COM ");

        Assert.Equal("user@example.com", email.Value);
    }

    [Fact]
    public void EmailAddress_Create_Throws_WhenInvalid()
    {
        Assert.Throws<ValidationException>(() => EmailAddress.Create("not-an-email"));
    }

    [Fact]
    public void MoneyAmount_Create_RoundsToTwoDecimals()
    {
        var amount = MoneyAmount.Create(12.345m);

        Assert.Equal(12.35m, amount.Value);
    }

    [Fact]
    public void MoneyAmount_Create_Throws_WhenZeroOrNegative()
    {
        Assert.Throws<ValidationException>(() => MoneyAmount.Create(0));
        Assert.Throws<ValidationException>(() => MoneyAmount.Create(-1));
    }
}
