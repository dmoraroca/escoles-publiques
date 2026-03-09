namespace Web.Models;
/// <summary>
/// Encapsulates the functional responsibility of column config within the application architecture.
/// </summary>
public class ColumnConfig
{
        public string PropertyName { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public bool IsSortable { get; set; } = true;

        public bool IsVisibleOnMobile { get; set; } = true;

        public string? Format { get; set; }

        public Func<object, string>? CustomRender { get; set; }

        public bool IsActionColumn { get; set; } = false;
}
/// <summary>
/// Encapsulates the functional responsibility of table action within the application architecture.
/// </summary>
public class TableAction
{
        public string ActionName { get; set; } = string.Empty;

        public string DisplayText { get; set; } = string.Empty;

        public string CssClass { get; set; } = "btn-secondary";

        public string? Icon { get; set; }

        public string? Controller { get; set; }

        public bool RequiresConfirmation { get; set; } = false;

        public string? ConfirmationMessage { get; set; }
}
/// <summary>
/// Encapsulates the functional responsibility of table view model within the application architecture.
/// </summary>
public class TableViewModel<T> where T : class
{
        public IEnumerable<T> Data { get; set; } = new List<T>();

        public List<ColumnConfig> Columns { get; set; } = new();

        public List<TableAction> Actions { get; set; } = new();

        public string EntityName { get; set; } = string.Empty;

        public string ControllerName { get; set; } = string.Empty;

        public bool HasSearch { get; set; } = true;

        public bool HasPagination { get; set; } = false;

        public int PageSize { get; set; } = 10;

        public int CurrentPage { get; set; } = 1;

        public int TotalItems { get; set; }

        public string IdPropertyName { get; set; } = "Id";

        public string CustomCssClass { get; set; } = string.Empty;

        public string EmptyMessage { get; set; } = "No hi ha dades per mostrar.";
}
