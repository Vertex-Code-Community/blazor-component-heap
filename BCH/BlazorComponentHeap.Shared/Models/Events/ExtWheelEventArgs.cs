using BlazorComponentHeap.Shared.Models.Markup;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorComponentHeap.Shared.Models.Events;

public class ExtWheelEventArgs : WheelEventArgs
{
    public float X { get; set; }
    public float Y { get; set; }

    public List<CoordsHolder> PathCoordinates { get; set; } = new();
}