using BlazorComponentHeap.Core.Models.Markup;

namespace BlazorComponentHeap.Core.Models.Events;

public class ExtTouchEventArgs : EventArgs
{
    public List<ExtTouchPoint> Touches { get; set; } = new();
    public List<CoordsHolder> PathCoordinates { get; set; } = new();
}