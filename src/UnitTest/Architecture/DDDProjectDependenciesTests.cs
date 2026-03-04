using System.Xml.Linq;

namespace UnitTest.Architecture;

public class DDDProjectDependenciesTests
{
    private static readonly IReadOnlyDictionary<string, HashSet<string>> AllowedDependencies =
        new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Domain"] = new(StringComparer.OrdinalIgnoreCase),
            ["Application"] = new(StringComparer.OrdinalIgnoreCase) { "Domain" },
            ["Infrastructure"] = new(StringComparer.OrdinalIgnoreCase) { "Application", "Domain" },
            ["Api"] = new(StringComparer.OrdinalIgnoreCase) { "Application", "Domain", "Infrastructure" },
            ["Web"] = new(StringComparer.OrdinalIgnoreCase) { "Application", "Domain", "Infrastructure" }
        };

    private static readonly IReadOnlyDictionary<string, HashSet<string>> RequiredDependencies =
        new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Application"] = new(StringComparer.OrdinalIgnoreCase) { "Domain" },
            ["Infrastructure"] = new(StringComparer.OrdinalIgnoreCase) { "Application", "Domain" },
            ["Api"] = new(StringComparer.OrdinalIgnoreCase) { "Application", "Domain", "Infrastructure" },
            ["Web"] = new(StringComparer.OrdinalIgnoreCase) { "Application", "Domain", "Infrastructure" }
        };

    [Fact]
    public void Project_References_Must_Respect_Ddd_Boundaries()
    {
        var repoRoot = FindRepoRoot();
        var violations = new List<string>();

        foreach (var project in AllowedDependencies.Keys)
        {
            var path = Path.Combine(repoRoot, "src", project, $"{project}.csproj");
            var referencedProjects = GetProjectReferences(path);
            var allowed = AllowedDependencies[project];

            foreach (var referenced in referencedProjects)
            {
                if (!allowed.Contains(referenced))
                {
                    violations.Add($"{project} -> {referenced} is not allowed.");
                }
            }
        }

        Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void Required_Ddd_Dependencies_Must_Be_Present()
    {
        var repoRoot = FindRepoRoot();
        var violations = new List<string>();

        foreach (var kvp in RequiredDependencies)
        {
            var project = kvp.Key;
            var required = kvp.Value;
            var path = Path.Combine(repoRoot, "src", project, $"{project}.csproj");
            var referencedProjects = GetProjectReferences(path);

            foreach (var dependency in required)
            {
                if (!referencedProjects.Contains(dependency))
                {
                    violations.Add($"{project} must reference {dependency}.");
                }
            }
        }

        Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
    }

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var solution = Path.Combine(current.FullName, "EscolesPubliques.sln");
            if (File.Exists(solution))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root (EscolesPubliques.sln).");
    }

    private static HashSet<string> GetProjectReferences(string csprojPath)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var document = XDocument.Load(csprojPath);
        var references = document.Descendants("ProjectReference")
            .Select(x => x.Attribute("Include")?.Value)
            .Where(x => !string.IsNullOrWhiteSpace(x));

        foreach (var reference in references)
        {
            var normalizedReference = reference!.Replace('\\', '/');
            var projectName = Path.GetFileNameWithoutExtension(normalizedReference);
            if (!string.IsNullOrWhiteSpace(projectName))
            {
                result.Add(projectName);
            }
        }

        return result;
    }
}
