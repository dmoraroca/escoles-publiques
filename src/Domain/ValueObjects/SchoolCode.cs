using System.Text.RegularExpressions;
using Domain.DomainExceptions;

namespace Domain.ValueObjects;

public readonly record struct SchoolCode
{
    private static readonly Regex AllowedPattern = new("^[A-Za-z0-9_-]{2,20}$", RegexOptions.Compiled);

    public string Value { get; }

    private SchoolCode(string value)
    {
        Value = value;
    }

    public static SchoolCode Create(string? raw)
    {
        var normalized = (raw ?? string.Empty).Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ValidationException("Code", "El codi de l'escola és obligatori");
        }

        if (!AllowedPattern.IsMatch(normalized))
        {
            throw new ValidationException("Code", "El codi de l'escola només admet caràcters alfanumèrics, '-' i '_', entre 2 i 20 caràcters");
        }

        return new SchoolCode(normalized);
    }

    public override string ToString() => Value;
}
