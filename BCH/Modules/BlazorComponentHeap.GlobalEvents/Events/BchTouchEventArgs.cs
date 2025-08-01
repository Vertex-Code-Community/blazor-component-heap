using BlazorComponentHeap.GlobalEvents.Models;

namespace BlazorComponentHeap.GlobalEvents.Events;

public class BchTouchEventArgs : EventArgs
{
    public List<BchTouchPoint> Touches { get; set; } = new();
    public List<ElementParameters> PathCoordinates { get; set; } = new();
}