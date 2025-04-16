namespace BlazorComponentHeap.Core.Models.Scheduler;

public class Day
{
    public DateTime DateTime { get; set; }
    public List<TimeIntersectionGroup> Groups { get; set; } = new();
}
