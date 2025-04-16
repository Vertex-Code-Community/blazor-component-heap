using BlazorComponentHeap.Core.Models.Datepicker;

namespace BlazorComponentHeap.Core.Models.Table;

public class TableFilterParameters
{
    public List<string> Filters { get; set; } = new();
    public string PropertyName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateRange DateRange { get; set; } = null!;
}
