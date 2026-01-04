using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class SchoolViewModel
{
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
    
    [StringLength(100, ErrorMessage = "L'àmbit no pot superar els 100 caràcters")]
    [Display(Name = "Àmbit")]
    public string? Scope { get; set; }
    
    [Display(Name = "Data de creació")]
    public DateTime CreatedAt { get; set; }
}
