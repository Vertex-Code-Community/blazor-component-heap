namespace Bch.Components.Calendar.Models;

public class DateRange
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public DateRange()
    {
        Start = DateTime.Now;
        End = DateTime.Now;
    }
}
