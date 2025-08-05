namespace Bch.Components.Scheduler.Models;

public class TimeIntersectionGroup
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public List<Appointment> Appointments { get; set; } = new();
}
