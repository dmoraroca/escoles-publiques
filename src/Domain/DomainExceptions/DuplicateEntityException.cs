namespace Domain.DomainExceptions;

/// <summary>
/// Exception thrown when attempting to create a duplicate entity
/// </summary>
public class DuplicateEntityException : DomainException
{
    public DuplicateEntityException(string entityName, string propertyName, object value)
        : base($"{entityName} with {propertyName} '{value}' already exists")
    {
    }

    public DuplicateEntityException(string message) : base(message)
    {
    }
}
