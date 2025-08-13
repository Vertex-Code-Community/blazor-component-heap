using Microsoft.AspNetCore.Components;
using Bch.Components.Calendar.Models;

namespace Bch.Integration.Pages.Pages.Datepicker;

public partial class DatepickerPage : ComponentBase
{
    private DateRange _range1 = new();
    private DateRange _range2 = new() { Start = DateTime.Today.AddDays(2) };
    private DateRange _range3 = new() { Start = DateTime.Today.AddDays(-3), End = DateTime.Today.AddDays(4) };

    private static string FormatRange(DateRange range)
    {
        string s = range.Start?.ToString("MM/dd/yyyy") ?? "--";
        string e = range.End?.ToString("MM/dd/yyyy") ?? "--";
        return $"{s} - {e}";
    }
}
