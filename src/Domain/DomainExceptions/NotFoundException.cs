namespace Domain.DomainExceptions;

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}
