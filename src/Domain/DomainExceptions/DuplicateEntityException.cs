namespace Domain.DomainExceptions;
/// <summary>
/// Encapsulates the functional responsibility of duplicate entity exception within the application architecture.
/// </summary>
public class DuplicateEntityException : DomainException
{
    public DuplicateEntityException(string entityName, string propertyName, object value)
        : base($"{entityName} with {propertyName} '{value}' already exists")
    {
    }
    /// <summary>
    /// Initializes a new instance of the DuplicateEntityException class with its required dependencies.
    /// </summary>
    public DuplicateEntityException(string message) : base(message)
    {
    }
}
