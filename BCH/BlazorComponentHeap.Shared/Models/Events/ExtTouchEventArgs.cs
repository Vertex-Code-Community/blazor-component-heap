using BlazorComponentHeap.Shared.Models.Markup;

namespace BlazorComponentHeap.Shared.Models.Events;

public class ExtTouchEventArgs : EventArgs
{
    public List<ExtTouchPoint> Touches { get; set; } = new();
    public List<CoordsHolder> PathCoordinates { get; set; } = new();
}