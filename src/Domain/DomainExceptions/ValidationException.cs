namespace Domain.DomainExceptions;
/// <summary>
/// Represents a domain validation failure with optional field-level error details.
/// </summary>
public class ValidationException : DomainException
{
    public Dictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Initializes a new validation exception with a plain error message.
    /// </summary>
    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred")
    {
        Errors = errors;
    }

    public ValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for {propertyName}: {errorMessage}")
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        };
    }
}
