using BlazorComponentHeap.Core.Models.Markup;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorComponentHeap.Core.Models.Events;

public class ExtWheelEventArgs : WheelEventArgs
{
    public float X { get; set; }
    public float Y { get; set; }

    public List<CoordsHolder> PathCoordinates { get; set; } = new();
}