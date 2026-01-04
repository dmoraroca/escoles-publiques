namespace Web.Models;

public class EntityModalConfig
{
    public string EntityName { get; set; } = string.Empty;
    public string EntityNamePlural { get; set; } = string.Empty;
    public string ModalId { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string IconClass { get; set; } = "bi-folder";
    public List<ModalField> Fields { get; set; } = new();
}

public class ModalField
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "text"; // text, select, checkbox, date, number
    public bool Required { get; set; }
    public int MaxLength { get; set; } = 100;
    public string Placeholder { get; set; } = string.Empty;
    public int ColumnSize { get; set; } = 12; // Bootstrap col-md-X
    public List<SelectOption>? Options { get; set; } // Per selects
}

public class SelectOption
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
