namespace Domain.DomainExceptions;
/// <summary>
/// Encapsulates the functional responsibility of not found exception within the application architecture.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found")
    {
    }
    /// <summary>
    /// Initializes a new instance of the NotFoundException class with its required dependencies.
    /// </summary>
    public NotFoundException(string message) : base(message)
    {
    }
}
