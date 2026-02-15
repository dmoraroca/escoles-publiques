using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public sealed class ManualController : Controller
{
    // Kept for backwards compatibility with conventional routing (/Manual/Index).
    // The public help center is now served by HelpController under /ajuda.
    public IActionResult Index() => Redirect("/ajuda/manual");

    // Old helper/types intentionally removed.
}
