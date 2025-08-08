using Microsoft.AspNetCore.Components;

namespace Bch.Integration.Pages.Pages.Calendar;

public partial class CalendarPage : ComponentBase
{
    private DateTime? _selectedDate1 = null;
    private DateTime? _selectedDate2 = DateTime.Now;
    private DateTime? _selectedDate3 = DateTime.Now;

    private void ShowDate()
    {
        Console.WriteLine($"Selected date 1: {_selectedDate1}");
        Console.WriteLine($"Selected date 2: {_selectedDate2}");
        Console.WriteLine($"Selected date 3: {_selectedDate3}");
    }
}
