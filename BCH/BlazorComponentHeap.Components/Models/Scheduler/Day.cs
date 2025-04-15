namespace BlazorComponentHeap.Components.Models.Scheduler;

internal class Day
{
    public DateTime DateTime { get; set; }
    public List<TimeIntersectionGroup> Groups { get; set; } = new();
}
