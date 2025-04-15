using BlazorComponentHeap.Shared.Enums.Table;

namespace BlazorComponentHeap.Shared.Models.Table;

public class ColumnInfo
{
    public string ColumnName { get; set; }
    public string PropertyName { get; set; }
    public float? Width { get; set; }
    public float? MinWidth { get; set; }
    public bool IsPx { get; set; }
    public List<ButtonConfig> Buttons { get; set; }
    public ColumnFilterType FilterType { get; set; }
    public bool IsSorted { get; set; }
}
