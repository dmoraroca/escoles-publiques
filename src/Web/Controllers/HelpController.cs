using Markdig;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
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
        if (ui.StartsWith("fr", StringComparison.OrdinalIgnoreCase)) return "fr";
        if (ui.StartsWith("ru", StringComparison.OrdinalIgnoreCase)) return "ru";
        if (ui.StartsWith("zh", StringComparison.OrdinalIgnoreCase)) return "zh";
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

            ("fr", "manual") => ("Manuel utilisateur", Path.Combine(baseDir, "manual.md")),
            ("fr", "funcional") => ("Document fonctionnel", Path.Combine(baseDir, "functional.md")),
            ("fr", "tecnic") => ("Document technique", Path.Combine(baseDir, "technical.md")),

            ("ru", "manual") => ("Руководство пользователя", Path.Combine(baseDir, "manual.md")),
            ("ru", "funcional") => ("Функциональный документ", Path.Combine(baseDir, "functional.md")),
            ("ru", "tecnic") => ("Технический документ", Path.Combine(baseDir, "technical.md")),

            ("zh", "manual") => ("用户手册", Path.Combine(baseDir, "manual.md")),
            ("zh", "funcional") => ("功能文档", Path.Combine(baseDir, "functional.md")),
            ("zh", "tecnic") => ("技术文档", Path.Combine(baseDir, "technical.md")),

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

    private byte[] BuildDocxFromMarkdown(string title, string markdown)
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
            EnsureStyles(mainPart);
            EnsureSettings(mainPart);

            var headerPart = mainPart.AddNewPart<HeaderPart>();
            headerPart.Header = new Header(
                new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Left }),
                    new Run(
                        new RunProperties(new Bold(), new FontSize { Val = "18" }),
                        new Text($"DavidGov | {title}"))));
            headerPart.Header.Save();

            var footerPart = mainPart.AddNewPart<FooterPart>();
            footerPart.Footer = BuildFooter();
            footerPart.Footer.Save();

            var sectionProps = new SectionProperties(
                new HeaderReference { Type = HeaderFooterValues.Default, Id = mainPart.GetIdOfPart(headerPart) },
                new FooterReference { Type = HeaderFooterValues.Default, Id = mainPart.GetIdOfPart(footerPart) },
                new PageSize { Width = 11906, Height = 16838 }, // A4
                new PageMargin
                {
                    Top = 1134,
                    Bottom = 1134,
                    Left = 1134,
                    Right = 1134,
                    Header = 708,
                    Footer = 708,
                    Gutter = 0
                });

            body.Append(CreateStyledParagraph(title, "Title", bold: true, size: "42"));
            body.Append(CreateStyledParagraph($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC", "Subtitle", italic: true, size: "20"));
            body.Append(CreateStyledParagraph("Table of contents", "Heading1", bold: true, size: "30"));
            var tocEntries = ExtractMarkdownHeadings(markdown);
            foreach (var (level, text) in tocEntries)
            {
                body.Append(CreateTocEntryParagraph(level, text));
            }
            body.Append(CreatePageBreakParagraph());
            body.Append(CreateStyledParagraph("Document", "Heading1", bold: true, size: "32"));

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
                    body.Append(CreateHeadingParagraph(text, level));
                    continue;
                }

                if (line.StartsWith("- ", StringComparison.Ordinal))
                {
                    body.Append(CreateBulletParagraph(line[2..].Trim()));
                    continue;
                }

                var numbered = Regex.Match(line, @"^\d+\.\s+(.+)$");
                if (numbered.Success)
                {
                    body.Append(CreateNumberedParagraph(line.Trim()));
                    continue;
                }

                var image = Regex.Match(line.Trim(), "^!\\[(.*?)\\]\\((.*?)\\)$");
                if (image.Success)
                {
                    var altText = image.Groups[1].Value.Trim();
                    var imagePath = image.Groups[2].Value.Trim();
                    var imageParagraph = CreateImageParagraph(mainPart, imagePath, altText);
                    if (imageParagraph is not null)
                    {
                        body.Append(imageParagraph);
                        continue;
                    }
                }

                body.Append(CreateBodyParagraph(line.Trim()));
            }

            body.Append(sectionProps);
            mainPart.Document.Save();
        }

        return stream.ToArray();
    }

    private static void EnsureStyles(MainDocumentPart mainPart)
    {
        var stylesPart = mainPart.StyleDefinitionsPart ?? mainPart.AddNewPart<StyleDefinitionsPart>();
        if (stylesPart.Styles is not null)
            return;

        stylesPart.Styles = new Styles(
            new Style(
                new StyleName { Val = "Normal" },
                new PrimaryStyle(),
                new StyleRunProperties(
                    new RunFonts { Ascii = "Calibri", HighAnsi = "Calibri" },
                    new FontSize { Val = "22" }))
            { Type = StyleValues.Paragraph, StyleId = "Normal", Default = true },
            new Style(
                new StyleName { Val = "Title" },
                new BasedOn { Val = "Normal" },
                new NextParagraphStyle { Val = "Normal" },
                new StyleRunProperties(
                    new RunFonts { Ascii = "Calibri", HighAnsi = "Calibri" },
                    new Bold(),
                    new FontSize { Val = "42" }))
            { Type = StyleValues.Paragraph, StyleId = "Title" },
            new Style(
                new StyleName { Val = "Subtitle" },
                new BasedOn { Val = "Normal" },
                new NextParagraphStyle { Val = "Normal" },
                new StyleRunProperties(
                    new RunFonts { Ascii = "Calibri", HighAnsi = "Calibri" },
                    new Italic(),
                    new Color { Val = "666666" },
                    new FontSize { Val = "20" }))
            { Type = StyleValues.Paragraph, StyleId = "Subtitle" },
            new Style(
                new StyleName { Val = "Heading 1" },
                new BasedOn { Val = "Normal" },
                new NextParagraphStyle { Val = "Normal" },
                new StyleParagraphProperties(
                    new OutlineLevel { Val = 0 }),
                new StyleRunProperties(
                    new Bold(),
                    new Color { Val = "1F2937" },
                    new FontSize { Val = "32" }))
            { Type = StyleValues.Paragraph, StyleId = "Heading1" },
            new Style(
                new StyleName { Val = "Heading 2" },
                new BasedOn { Val = "Normal" },
                new NextParagraphStyle { Val = "Normal" },
                new StyleParagraphProperties(
                    new OutlineLevel { Val = 1 }),
                new StyleRunProperties(
                    new Bold(),
                    new Color { Val = "374151" },
                    new FontSize { Val = "28" }))
            { Type = StyleValues.Paragraph, StyleId = "Heading2" },
            new Style(
                new StyleName { Val = "Heading 3" },
                new BasedOn { Val = "Normal" },
                new NextParagraphStyle { Val = "Normal" },
                new StyleParagraphProperties(
                    new OutlineLevel { Val = 2 }),
                new StyleRunProperties(
                    new Bold(),
                    new Color { Val = "374151" },
                    new FontSize { Val = "24" }))
            { Type = StyleValues.Paragraph, StyleId = "Heading3" });
        stylesPart.Styles.Save();
    }

    private static void EnsureSettings(MainDocumentPart mainPart)
    {
        var settingsPart = mainPart.DocumentSettingsPart ?? mainPart.AddNewPart<DocumentSettingsPart>();
        settingsPart.Settings ??= new Settings();

        if (!settingsPart.Settings.Elements<UpdateFieldsOnOpen>().Any())
            settingsPart.Settings.Append(new UpdateFieldsOnOpen { Val = true });

        settingsPart.Settings.Save();
    }

    private static Footer BuildFooter()
    {
        return new Footer(
            new Paragraph(
                new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                new Run(new RunProperties(new FontSize { Val = "18" }), new Text("Page ")),
                CreateSimpleFieldRun(" PAGE "),
                new Run(new RunProperties(new FontSize { Val = "18" }), new Text(" of ")),
                CreateSimpleFieldRun(" NUMPAGES ")));
    }

    private static Run CreateSimpleFieldRun(string code)
    {
        return new Run(
            new FieldChar { FieldCharType = FieldCharValues.Begin },
            new FieldCode(code),
            new FieldChar { FieldCharType = FieldCharValues.Separate },
            new Text("1"),
            new FieldChar { FieldCharType = FieldCharValues.End });
    }

    private static List<(int level, string text)> ExtractMarkdownHeadings(string markdown)
    {
        var entries = new List<(int level, string text)>();
        var lines = markdown.Replace("\r\n", "\n").Split('\n');
        foreach (var raw in lines)
        {
            var line = (raw ?? string.Empty).Trim();
            var heading = Regex.Match(line, @"^(#{1,3})\s+(.+)$");
            if (!heading.Success)
                continue;

            var level = heading.Groups[1].Value.Length;
            var text = heading.Groups[2].Value.Trim();
            entries.Add((level, text));
        }

        return entries;
    }

    private static Paragraph CreateTocEntryParagraph(int level, string text)
    {
        var safeLevel = Math.Max(1, Math.Min(level, 3));
        var indent = safeLevel switch
        {
            1 => "0",
            2 => "360",
            _ => "720"
        };

        var paragraph = CreateStyledParagraph(text, "Normal", size: "22");
        paragraph.ParagraphProperties ??= new ParagraphProperties();
        paragraph.ParagraphProperties.Indentation = new Indentation { Left = indent };
        return paragraph;
    }

    private static Paragraph CreatePageBreakParagraph()
    {
        return new Paragraph(new Run(new Break { Type = BreakValues.Page }));
    }

    private static Paragraph CreateBodyParagraph(string text)
    {
        return CreateStyledParagraph(text, "Normal", size: "22");
    }

    private static Paragraph CreateBulletParagraph(string text)
    {
        var paragraph = CreateStyledParagraph("• " + text, "Normal", size: "22");
        paragraph.ParagraphProperties ??= new ParagraphProperties();
        paragraph.ParagraphProperties.Indentation = new Indentation { Left = "360", Hanging = "180" };
        return paragraph;
    }

    private static Paragraph CreateNumberedParagraph(string text)
    {
        var paragraph = CreateStyledParagraph(text, "Normal", size: "22");
        paragraph.ParagraphProperties ??= new ParagraphProperties();
        paragraph.ParagraphProperties.Indentation = new Indentation { Left = "360", Hanging = "120" };
        return paragraph;
    }

    private static Paragraph CreateHeadingParagraph(string text, int level)
    {
        var style = level switch
        {
            1 => "Heading1",
            2 => "Heading2",
            _ => "Heading3"
        };

        var size = level switch
        {
            1 => "32",
            2 => "28",
            3 => "24",
            _ => "22"
        };

        var paragraph = CreateStyledParagraph(text, style, bold: true, size: size);
        paragraph.ParagraphProperties ??= new ParagraphProperties();
        paragraph.ParagraphProperties.OutlineLevel = new OutlineLevel { Val = Math.Min(level - 1, 8) };
        return paragraph;
    }

    private static Paragraph CreateStyledParagraph(
        string text,
        string styleId,
        bool bold = false,
        bool italic = false,
        string size = "22")
    {
        var runProperties = new RunProperties(new FontSize { Val = size });
        if (bold) runProperties.Append(new Bold());
        if (italic) runProperties.Append(new Italic());

        var run = new Run(runProperties, new Text(text) { Space = SpaceProcessingModeValues.Preserve });
        var paragraph = new Paragraph(run);
        paragraph.ParagraphProperties = new ParagraphProperties(
            new ParagraphStyleId { Val = styleId },
            new SpacingBetweenLines { Before = "40", After = "120", Line = "280", LineRule = LineSpacingRuleValues.Auto });
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
            new ParagraphStyleId { Val = "Normal" },
            new Indentation { Left = "320", Right = "160" },
            new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "F3F4F6" },
            new SpacingBetweenLines { After = "60", Line = "260", LineRule = LineSpacingRuleValues.Auto });
        return paragraph;
    }

    private Paragraph? CreateImageParagraph(MainDocumentPart mainPart, string markdownPath, string altText)
    {
        var fullPath = ResolveImagePath(markdownPath);
        if (fullPath is null || !System.IO.File.Exists(fullPath))
            return null;

        var extension = Path.GetExtension(fullPath).ToLowerInvariant();
        var partType = extension switch
        {
            ".png" => ImagePartType.Png,
            ".jpg" => ImagePartType.Jpeg,
            ".jpeg" => ImagePartType.Jpeg,
            ".gif" => ImagePartType.Gif,
            ".bmp" => ImagePartType.Bmp,
            _ => ImagePartType.Png
        };

        var imagePart = mainPart.AddImagePart(partType);
        using (var fs = System.IO.File.OpenRead(fullPath))
            imagePart.FeedData(fs);

        var relationshipId = mainPart.GetIdOfPart(imagePart);
        var drawing = CreateInlineImageDrawing(relationshipId, altText);
        return new Paragraph(
            new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
            new Run(drawing));
    }

    private string? ResolveImagePath(string markdownPath)
    {
        if (string.IsNullOrWhiteSpace(markdownPath))
            return null;

        var path = markdownPath.Trim();

        if (path.StartsWith("/ajuda/", StringComparison.OrdinalIgnoreCase))
        {
            var rel = path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(_env.WebRootPath, rel);
        }

        if (path.StartsWith("../", StringComparison.Ordinal))
        {
            var rel = path.Replace('/', Path.DirectorySeparatorChar);
            var docsBase = Path.Combine(_env.ContentRootPath, "..", "docs");
            return Path.GetFullPath(Path.Combine(docsBase, rel));
        }

        if (Path.IsPathRooted(path))
            return path;

        return Path.Combine(_env.WebRootPath, path.Replace('/', Path.DirectorySeparatorChar));
    }

    private static Drawing CreateInlineImageDrawing(string relationshipId, string altText)
    {
        // ~6.2 x 3.6 inches keeps screenshots readable on A4.
        const long widthEmu = 5669280L;
        const long heightEmu = 3291840L;
        var name = string.IsNullOrWhiteSpace(altText) ? "Screenshot" : altText;
        var docPropId = (uint)Random.Shared.Next(1000, 900000);

        return new Drawing(
            new DW.Inline(
                new DW.Extent { Cx = widthEmu, Cy = heightEmu },
                new DW.EffectExtent
                {
                    LeftEdge = 0L,
                    TopEdge = 0L,
                    RightEdge = 0L,
                    BottomEdge = 0L
                },
                new DW.DocProperties
                {
                    Id = docPropId,
                    Name = name
                },
                new DW.NonVisualGraphicFrameDrawingProperties(
                    new A.GraphicFrameLocks { NoChangeAspect = true }),
                new A.Graphic(
                    new A.GraphicData(
                        new PIC.Picture(
                            new PIC.NonVisualPictureProperties(
                                new PIC.NonVisualDrawingProperties
                                {
                                    Id = 0U,
                                    Name = name
                                },
                                new PIC.NonVisualPictureDrawingProperties()),
                            new PIC.BlipFill(
                                new A.Blip
                                {
                                    Embed = relationshipId,
                                    CompressionState = A.BlipCompressionValues.Print
                                },
                                new A.Stretch(new A.FillRectangle())),
                            new PIC.ShapeProperties(
                                new A.Transform2D(
                                    new A.Offset { X = 0L, Y = 0L },
                                    new A.Extents { Cx = widthEmu, Cy = heightEmu }),
                                new A.PresetGeometry(new A.AdjustValueList())
                                {
                                    Preset = A.ShapeTypeValues.Rectangle
                                }))
                    )
                    {
                        Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture"
                    }))
            {
                DistanceFromTop = 0U,
                DistanceFromBottom = 0U,
                DistanceFromLeft = 0U,
                DistanceFromRight = 0U,
                EditId = "50D07946"
            });
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
