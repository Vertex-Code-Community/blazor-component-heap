namespace Bch.Components.Table.Models;

public class ColumnInfo
{
    public required string ColumnName { get; set; }
    public required string PropertyName { get; set; }
    public float? Width { get; set; }
    public float? MinWidth { get; set; }
    public bool IsPx { get; set; }
    public List<ButtonConfig> Buttons { get; set; } = new();
    public ColumnFilterType FilterType { get; set; }
    public bool IsSorted { get; set; }
}
