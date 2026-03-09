namespace Web.Models;
/// <summary>
/// Encapsulates the functional responsibility of error view model within the application architecture.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
