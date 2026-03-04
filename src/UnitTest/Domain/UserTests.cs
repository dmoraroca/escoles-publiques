using Domain.Entities;
using Xunit;

namespace UnitTest.DomainTests;

public class UserTests
{
    [Fact]
    public void IsAdmin_ReturnsTrueOnlyForAdmRole()
    {
        var user = new User { Role = "ADM" };
        Assert.True(user.IsAdmin());
        Assert.False(user.IsUser());
    }

    [Fact]
    public void IsUser_ReturnsTrueOnlyForUserRole()
    {
        var user = new User { Role = "USER" };
        Assert.True(user.IsUser());
        Assert.False(user.IsAdmin());
    }
}
