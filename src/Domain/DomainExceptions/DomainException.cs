namespace Domain.DomainExceptions;
/// <summary>
/// Encapsulates the functional responsibility of domain exception within the application architecture.
/// </summary>
public abstract class DomainException : Exception
{
            /// <summary>
            /// Initializes a new instance of the DomainException class with its required dependencies.
            /// </summary>
            protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
