namespace Web.Models;

/// <summary>
/// Model de vista per mostrar errors i l'identificador de la petició.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
