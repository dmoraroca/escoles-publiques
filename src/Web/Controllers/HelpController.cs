using Markdig;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

// Public help center: user manual, technical doc, functional doc.
public sealed class HelpController : Controller
{
    private readonly IWebHostEnvironment _env;
    private static readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public HelpController(IWebHostEnvironment env)
    {
        _env = env;
    }

    // Aliases
    [HttpGet("/help")]
    [HttpGet("/ajuda")]
    public IActionResult Index()
    {
        var lang = CurrentLang();
        return View(new HelpIndexViewModel(lang));
    }

    // Docs
    [HttpGet("/help/{doc}")]
    [HttpGet("/ajuda/{doc}")]
    public IActionResult Doc(string doc)
    {
        var lang = CurrentLang();
        var (title, path) = ResolveDoc(lang, doc);
        if (path is null)
            return NotFound();

        if (!System.IO.File.Exists(path))
            return NotFound();

        var markdown = System.IO.File.ReadAllText(path);
        markdown = StripTrailingScreenshotIndex(markdown);

        var html = Markdown.ToHtml(markdown, _pipeline);

        return View("Doc", new HelpDocViewModel
        {
            Lang = lang,
            Doc = doc.ToLowerInvariant(),
            Title = title,
            Html = html
        });
    }

    // Backwards-compat: old /manual route now goes through /ajuda/manual.
    [HttpGet("/manual")]
    [HttpGet("/manual/{lang}")]
    public IActionResult ManualAlias()
    {
        return Redirect("/ajuda/manual");
    }

    private string CurrentLang()
    {
        // Align with localization config: culture is persisted in cookie and used as CurrentUICulture.
        var ui = System.Globalization.CultureInfo.CurrentUICulture.Name;
        if (ui.StartsWith("es", StringComparison.OrdinalIgnoreCase)) return "es";
        if (ui.StartsWith("en", StringComparison.OrdinalIgnoreCase)) return "en";
        if (ui.StartsWith("de", StringComparison.OrdinalIgnoreCase)) return "de";
        return "ca";
    }

    private (string title, string? path) ResolveDoc(string lang, string docRaw)
    {
        var doc = (docRaw ?? string.Empty).Trim().ToLowerInvariant();
        var baseDir = Path.Combine(_env.ContentRootPath, "HelpDocs", lang);

        // Map doc keys to per-language filenames.
        return (lang, doc) switch
        {
            ("ca", "manual") => ("Manual d'usuari", Path.Combine(baseDir, "manual.md")),
            ("ca", "funcional") => ("Document funcional", Path.Combine(baseDir, "funcional.md")),
            ("ca", "tecnic") => ("Document tècnic", Path.Combine(baseDir, "tecnic.md")),

            ("es", "manual") => ("Manual de usuario", Path.Combine(baseDir, "manual.md")),
            ("es", "funcional") => ("Documento funcional", Path.Combine(baseDir, "funcional.md")),
            ("es", "tecnic") => ("Documento técnico", Path.Combine(baseDir, "tecnic.md")),

            ("en", "manual") => ("User manual", Path.Combine(baseDir, "manual.md")),
            ("en", "funcional") => ("Functional document", Path.Combine(baseDir, "functional.md")),
            ("en", "tecnic") => ("Technical document", Path.Combine(baseDir, "technical.md")),

            // German filenames in repo are non-standard; keep mapping explicit.
            ("de", "manual") => ("Benutzerhandbuch", Path.Combine(baseDir, "manual.md")),
            ("de", "funcional") => ("Fachliches Dokument", Path.Combine(baseDir, "fachlich.md")),
            ("de", "tecnic") => ("Technisches Dokument", Path.Combine(baseDir, "technisch.md")),

            _ => ("", null)
        };
    }

    private static string StripTrailingScreenshotIndex(string markdown)
    {
        // Manuals contain an appendix/index with raw file paths that looks bad on the web.
        // Remove it to keep the page presentable (images are already embedded above).
        var markers = new[]
        {
            "\n## Appendix:",
            "\n## Appendix",
            "\n## Anexo",
            "\n## Anhang",
            "\n## Annex",
            "\n## Annex:"
        };

        var cut = -1;
        foreach (var m in markers)
        {
            var idx = markdown.IndexOf(m, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0 && (cut < 0 || idx < cut))
                cut = idx;
        }

        if (cut < 0)
            return markdown;

        return markdown.Substring(0, cut).TrimEnd() + "\n";
    }

    public sealed record HelpIndexViewModel(string Lang);

    public sealed class HelpDocViewModel
    {
        public required string Lang { get; init; }
        public required string Doc { get; init; }
        public required string Title { get; init; }
        public required string Html { get; init; }
    }
}

