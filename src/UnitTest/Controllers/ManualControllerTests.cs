using Microsoft.AspNetCore.Mvc;
using Web.Controllers;

namespace UnitTest.Controllers;

public class ManualControllerTests
{
    [Fact]
    public void Index_RedirectsToAjudaManual()
    {
        var controller = new ManualController();

        var result = controller.Index();

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/ajuda/manual", redirect.Url);
    }
}
