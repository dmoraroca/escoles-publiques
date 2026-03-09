using System.Net.Mail;
using Domain.DomainExceptions;

namespace Domain.ValueObjects;

public readonly record struct EmailAddress
{
    public string Value { get; }
        /// <summary>
        /// Initializes a new instance of the EmailAddress class with its required dependencies.
        /// </summary>
        private EmailAddress(string value)
    {
        Value = value;
    }
        /// <summary>
        /// Creates a new resource by applying the required business rules.
        /// </summary>
        public static EmailAddress Create(string? raw)
    {
        var normalized = (raw ?? string.Empty).Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ValidationException("Email", "El correu electrònic és obligatori");
        }

        try
        {
            var mail = new MailAddress(normalized);
            if (!string.Equals(mail.Address, normalized, StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException("Email", "El correu electrònic no és vàlid");
            }
        }
        catch (FormatException)
        {
            throw new ValidationException("Email", "El correu electrònic no és vàlid");
        }

        return new EmailAddress(normalized);
    }

        public override string ToString() => Value;
}
