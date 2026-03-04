using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers;

namespace UnitTest.Controllers;

public class HelpControllerTests
{
    [Fact]
    public void Index_ReturnsViewWithCurrentLanguage()
    {
        using var scope = new CultureScope("es-ES");
        var controller = CreateControllerWithTempRoot(out _);

        var result = controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<HelpController.HelpIndexViewModel>(view.Model);
        Assert.Equal("es", model.Lang);
    }

    [Fact]
    public void ManualAlias_RedirectsToAjudaManual()
    {
        var controller = CreateControllerWithTempRoot(out _);

        var result = controller.ManualAlias();

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/ajuda/manual", redirect.Url);
    }

    [Fact]
    public void Doc_ReturnsNotFound_WhenDocKeyIsUnknown()
    {
        var controller = CreateControllerWithTempRoot(out _);

        var result = controller.Doc("unknown");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Doc_ReturnsNotFound_WhenFileDoesNotExist()
    {
        using var scope = new CultureScope("ca-ES");
        var controller = CreateControllerWithTempRoot(out var root);
        var docPath = Path.Combine(root, "HelpDocs", "ca", "manual.md");
        if (File.Exists(docPath))
        {
            File.Delete(docPath);
        }

        var result = controller.Doc("manual");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Doc_ReturnsDocView_WithHtmlAndDocxUrl()
    {
        using var scope = new CultureScope("ca-ES");
        var controller = CreateControllerWithTempRoot(out var root);
        WriteDoc(root, "ca", "manual.md", "# Titol\n\nText\n\n## Appendix:\nindex");

        var result = controller.Doc("manual");

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Doc", view.ViewName);
        var model = Assert.IsType<HelpController.HelpDocViewModel>(view.Model);
        Assert.Equal("ca", model.Lang);
        Assert.Equal("manual", model.Doc);
        Assert.Equal("Manual d'usuari", model.Title);
        Assert.Contains("<h1 id=\"titol\">Titol</h1>", model.Html);
        Assert.DoesNotContain("Appendix", model.Html);
        Assert.StartsWith("/ajuda/manual/docx?culture=ca-ES&ui-culture=ca-ES", model.DocxUrl, StringComparison.Ordinal);
    }

    [Fact]
    public void Docx_ReturnsNotFound_WhenFileDoesNotExist()
    {
        using var scope = new CultureScope("fr-FR");
        var controller = CreateControllerWithTempRoot(out _);

        var result = controller.Docx("manual");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Docx_ReturnsWordFile_WhenDocumentExists()
    {
        using var scope = new CultureScope("en-US");
        var controller = CreateControllerWithTempRoot(out var root);
        WriteDoc(root, "en", "manual.md", "# User manual\n\nSimple content.");

        var result = controller.Docx("manual");

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/vnd.openxmlformats-officedocument.wordprocessingml.document", file.ContentType);
        Assert.Equal("manual-en.docx", file.FileDownloadName);
        Assert.NotNull(file.FileContents);
        Assert.NotEmpty(file.FileContents);
    }

    private static HelpController CreateControllerWithTempRoot(out string root)
    {
        root = Path.Combine(Path.GetTempPath(), "help-controller-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        var env = new Mock<IWebHostEnvironment>();
        env.SetupGet(e => e.ContentRootPath).Returns(root);

        var controller = new HelpController(env.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
            }
        };

        return controller;
    }

    private static void WriteDoc(string root, string lang, string file, string markdown)
    {
        var dir = Path.Combine(root, "HelpDocs", lang);
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, file), markdown);
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _oldCulture;
        private readonly CultureInfo _oldUiCulture;

        public CultureScope(string culture)
        {
            _oldCulture = CultureInfo.CurrentCulture;
            _oldUiCulture = CultureInfo.CurrentUICulture;
            var newCulture = CultureInfo.GetCultureInfo(culture);
            CultureInfo.CurrentCulture = newCulture;
            CultureInfo.CurrentUICulture = newCulture;
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = _oldCulture;
            CultureInfo.CurrentUICulture = _oldUiCulture;
        }
    }
}
