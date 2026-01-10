using System.ComponentModel.DataAnnotations;

namespace Web.Models;

/// <summary>
/// Model de vista per gestionar les dades d'una escola.
/// </summary>
public class SchoolViewModel
{
    // Per compatibilitat amb vistes antigues
    public string? Scope { get; set; }
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El codi de l'escola és obligatori")]
    [StringLength(20, ErrorMessage = "El codi no pot superar els 20 caràcters")]
    [Display(Name = "Codi")]
    public string Code { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El nom de l'escola és obligatori")]
    [StringLength(200, ErrorMessage = "El nom no pot superar els 200 caràcters")]
    [Display(Name = "Nom")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(100, ErrorMessage = "La ciutat no pot superar els 100 caràcters")]
    [Display(Name = "Ciutat")]
    public string City { get; set; } = string.Empty;
    
    [Display(Name = "Escola favorita")]
    public bool IsFavorite { get; set; }
    
    [Display(Name = "Àmbit")]
    public long? ScopeId { get; set; }

    [Display(Name = "Nom de l'àmbit")]
    public string? ScopeName { get; set; }
    
    [Display(Name = "Data de creació")]
    public DateTime CreatedAt { get; set; }
}
