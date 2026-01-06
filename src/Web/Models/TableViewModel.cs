namespace Web.Models;

/// <summary>
/// Model de vista per gestionar taules genèriques amb configuració de columnes i files.
/// </summary>


/// <summary>
/// Configuració per a una columna de la taula
/// </summary>
public class ColumnConfig
{
    /// <summary>
    /// Nom de la propietat del model (ex: "Name", "City")
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;
    
    /// <summary>
    /// Etiqueta a mostrar a la capçalera (ex: "Nom", "Ciutat")
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Si la columna és ordenable
    /// </summary>
    public bool IsSortable { get; set; } = true;
    
    /// <summary>
    /// Si la columna és visible en mòbil
    /// </summary>
    public bool IsVisibleOnMobile { get; set; } = true;
    
    /// <summary>
    /// Format personalitzat (ex: "dd/MM/yyyy" per dates, "C2" per moneda)
    /// </summary>
    public string? Format { get; set; }
    
    /// <summary>
    /// Funció personalitzada per renderitzar la cel·la (opcional)
    /// </summary>
    public Func<object, string>? CustomRender { get; set; }
    
    /// <summary>
    /// Indica si és una columna d'accions (Edit, Delete, etc.)
    /// </summary>
    public bool IsActionColumn { get; set; } = false;
}

/// <summary>
/// Configuració per una acció de la taula (Edit, Delete, Details)
/// </summary>
public class TableAction
{
    /// <summary>
    /// Nom de l'acció (ex: "Edit", "Delete", "Details")
    /// </summary>
    public string ActionName { get; set; } = string.Empty;
    
    /// <summary>
    /// Text a mostrar (ex: "Editar", "Esborrar")
    /// </summary>
    public string DisplayText { get; set; } = string.Empty;
    
    /// <summary>
    /// Classe CSS del botó (ex: "btn-primary", "btn-danger")
    /// </summary>
    public string CssClass { get; set; } = "btn-secondary";
    
    /// <summary>
    /// Icona (opcional) - ex: "✏️", "🗑️", "👁️"
    /// </summary>
    public string? Icon { get; set; }
    
    /// <summary>
    /// Controller personalitzat (si és diferent del context actual)
    /// </summary>
    public string? Controller { get; set; }
    
    /// <summary>
    /// Si requereix confirmació abans d'executar
    /// </summary>
    public bool RequiresConfirmation { get; set; } = false;
    
    /// <summary>
    /// Missatge de confirmació
    /// </summary>
    public string? ConfirmationMessage { get; set; }
}

/// <summary>
/// Model principal per configurar una taula genèrica
/// </summary>
public class TableViewModel<T> where T : class
{
    /// <summary>
    /// Dades a mostrar a la taula
    /// </summary>
    public IEnumerable<T> Data { get; set; } = new List<T>();
    
    /// <summary>
    /// Configuració de les columnes
    /// </summary>
    public List<ColumnConfig> Columns { get; set; } = new();
    
    /// <summary>
    /// Accions disponibles per cada fila
    /// </summary>
    public List<TableAction> Actions { get; set; } = new();
    
    /// <summary>
    /// Nom de l'entitat (ex: "Escola", "Alumne")
    /// </summary>
    public string EntityName { get; set; } = string.Empty;
    
    /// <summary>
    /// Nom del controller per les accions
    /// </summary>
    public string ControllerName { get; set; } = string.Empty;
    
    /// <summary>
    /// Si la taula té cerca activada
    /// </summary>
    public bool HasSearch { get; set; } = true;
    
    /// <summary>
    /// Si la taula té paginació activada
    /// </summary>
    public bool HasPagination { get; set; } = false;
    
    /// <summary>
    /// Elements per pàgina
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// Pàgina actual
    /// </summary>
    public int CurrentPage { get; set; } = 1;
    
    /// <summary>
    /// Total d'elements
    /// </summary>
    public int TotalItems { get; set; }
    
    /// <summary>
    /// Propietat per identificar cada fila (normalment "Id")
    /// </summary>
    public string IdPropertyName { get; set; } = "Id";
    
    /// <summary>
    /// Classe CSS personalitzada per la taula
    /// </summary>
    public string CustomCssClass { get; set; } = string.Empty;
    
    /// <summary>
    /// Missatge quan no hi ha dades
    /// </summary>
    public string EmptyMessage { get; set; } = "No hi ha dades per mostrar.";
}
