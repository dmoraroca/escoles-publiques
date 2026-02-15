using Markdig;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public sealed class ManualController : Controller
{
    private readonly IWebHostEnvironment _env;
    private static readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public ManualController(IWebHostEnvironment env)
    {
        _env = env;
    }

    // Public manual, accessible without authentication.
    [HttpGet("/manual")]
    [HttpGet("/manual/{lang}")]
    public IActionResult Index(string? lang)
    {
        var normalized = NormalizeLang(lang) ?? NormalizeLang(System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName) ?? "ca";

        var manualFileName = normalized switch
        {
            "ca" => "manual-ca.md",
            "es" => "manual-es.md",
            "en" => "manual-en.md",
            "de" => "manual-de.md",
            _ => "manual-ca.md"
        };

        var manualPath = Path.Combine(_env.ContentRootPath, "Manual", manualFileName);
        if (!System.IO.File.Exists(manualPath))
            return NotFound();

        var markdown = System.IO.File.ReadAllText(manualPath);
        var html = Markdown.ToHtml(markdown, _pipeline);

        return View("Index", new ManualViewModel
        {
            Lang = normalized,
            Title = "Manual",
            Html = html
        });
    }

    private static string? NormalizeLang(string? lang)
    {
        if (string.IsNullOrWhiteSpace(lang))
            return null;

        var l = lang.Trim().ToLowerInvariant();
        return l switch
        {
            "ca" => "ca",
            "es" => "es",
            "en" => "en",
            "de" => "de",
            _ => null
        };
    }

    public sealed class ManualViewModel
    {
        public required string Lang { get; init; }
        public required string Title { get; init; }
        public required string Html { get; init; }
    }
}

