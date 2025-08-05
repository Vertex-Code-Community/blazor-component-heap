using Bch.Components.Calendar.Models;

namespace Bch.Components.Table.Models;

public class TableFilterParameters
{
    public List<string> Filters { get; set; } = new();
    public required string PropertyName { get; set; }
    public DateTime? Date { get; set; }
    public DateRange? DateRange { get; set; }
}
