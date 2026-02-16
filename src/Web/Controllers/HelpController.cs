using Markdig;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Web.Controllers;

// Public help center: user manual, technical doc, functional doc.
[AllowAnonymous]
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
        var docKey = NormalizeDocKey(doc);
        var (title, path) = ResolveDoc(lang, docKey);
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
            Doc = docKey,
            Title = title,
            Html = html,
            DocxUrl = BuildCultureUrl($"/ajuda/{docKey}/docx")
        });
    }

    [HttpGet("/help/{doc}/docx")]
    [HttpGet("/ajuda/{doc}/docx")]
    public IActionResult Docx(string doc)
    {
        var lang = CurrentLang();
        var docKey = NormalizeDocKey(doc);
        var (title, path) = ResolveDoc(lang, docKey);
        if (path is null || !System.IO.File.Exists(path))
            return NotFound();

        var markdown = System.IO.File.ReadAllText(path);
        markdown = StripTrailingScreenshotIndex(markdown);
        var content = BuildDocxFromMarkdown(title, markdown);

        var fileName = $"{docKey}-{lang}.docx";
        return File(
            content,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            fileName);
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

    private (string title, string? path) ResolveDoc(string lang, string docKey)
    {
        var baseDir = Path.Combine(_env.ContentRootPath, "HelpDocs", lang);

        // Map doc keys to per-language filenames.
        return (lang, docKey) switch
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

    private static string NormalizeDocKey(string? doc)
    {
        var d = (doc ?? string.Empty).Trim().ToLowerInvariant();
        return d switch
        {
            "manual" => "manual",
            "funcional" => "funcional",
            "functional" => "funcional",
            "fachlich" => "funcional",
            "tecnic" => "tecnic",
            "tecnico" => "tecnic",
            "technical" => "tecnic",
            "technisch" => "tecnic",
            _ => d
        };
    }

    private string BuildCultureUrl(string basePath)
    {
        var culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        return Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(
            basePath,
            new Dictionary<string, string?>
            {
                ["culture"] = culture,
                ["ui-culture"] = culture
            });
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

    private static byte[] BuildDocxFromMarkdown(string title, string markdown)
    {
        using var stream = new MemoryStream();
        using (var document = WordprocessingDocument.Create(
                   stream,
                   WordprocessingDocumentType.Document,
                   true))
        {
            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document(new Body());
            var body = mainPart.Document.Body!;

            body.Append(CreateParagraph(title, bold: true, size: "36"));
            body.Append(CreateParagraph($"Generat: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC", italic: true, size: "20"));
            body.Append(new Paragraph(new Run(new Text(string.Empty))));

            var lines = markdown.Replace("\r\n", "\n").Split('\n');
            var inCode = false;
            foreach (var raw in lines)
            {
                var line = raw ?? string.Empty;
                if (line.TrimStart().StartsWith("```", StringComparison.Ordinal))
                {
                    inCode = !inCode;
                    continue;
                }

                if (inCode)
                {
                    body.Append(CreateCodeParagraph(line));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    body.Append(new Paragraph(new Run(new Text(string.Empty))));
                    continue;
                }

                var heading = Regex.Match(line, @"^(#{1,6})\s+(.+)$");
                if (heading.Success)
                {
                    var level = heading.Groups[1].Value.Length;
                    var text = heading.Groups[2].Value.Trim();
                    var size = level switch
                    {
                        1 => "32",
                        2 => "28",
                        3 => "24",
                        _ => "22"
                    };
                    body.Append(CreateParagraph(text, bold: true, size: size));
                    continue;
                }

                if (line.StartsWith("- ", StringComparison.Ordinal))
                {
                    body.Append(CreateParagraph("• " + line[2..].Trim(), size: "22"));
                    continue;
                }

                var numbered = Regex.Match(line, @"^\d+\.\s+(.+)$");
                if (numbered.Success)
                {
                    body.Append(CreateParagraph(line.Trim(), size: "22"));
                    continue;
                }

                body.Append(CreateParagraph(line.Trim(), size: "22"));
            }

            mainPart.Document.Save();
        }

        return stream.ToArray();
    }

    private static Paragraph CreateParagraph(string text, bool bold = false, bool italic = false, string size = "22")
    {
        var runProperties = new RunProperties(new FontSize { Val = size });
        if (bold) runProperties.Append(new Bold());
        if (italic) runProperties.Append(new Italic());

        var run = new Run(runProperties, new Text(text) { Space = SpaceProcessingModeValues.Preserve });
        var paragraph = new Paragraph(run);
        paragraph.ParagraphProperties = new ParagraphProperties(
            new SpacingBetweenLines { After = "120", Line = "280", LineRule = LineSpacingRuleValues.Auto });
        return paragraph;
    }

    private static Paragraph CreateCodeParagraph(string text)
    {
        var runProps = new RunProperties(
            new RunFonts { Ascii = "Consolas", HighAnsi = "Consolas" },
            new FontSize { Val = "20" });

        var paragraph = new Paragraph(
            new Run(runProps, new Text(text) { Space = SpaceProcessingModeValues.Preserve }));

        paragraph.ParagraphProperties = new ParagraphProperties(
            new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "F3F4F6" },
            new SpacingBetweenLines { After = "60", Line = "260", LineRule = LineSpacingRuleValues.Auto });
        return paragraph;
    }

    public sealed record HelpIndexViewModel(string Lang);

    public sealed class HelpDocViewModel
    {
        public required string Lang { get; init; }
        public required string Doc { get; init; }
        public required string Title { get; init; }
        public required string Html { get; init; }
        public required string DocxUrl { get; init; }
    }
}
