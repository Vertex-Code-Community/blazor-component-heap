using Microsoft.AspNetCore.Components.Web;
using BlazorComponentHeap.GlobalEvents.Models;

namespace BlazorComponentHeap.GlobalEvents.Events;

public class BchWheelEventArgs : WheelEventArgs
{
    public float X { get; set; }
    public float Y { get; set; }

    public List<ElementParameters> PathCoordinates { get; set; } = new();
}