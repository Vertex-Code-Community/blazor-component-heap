using Microsoft.AspNetCore.Components.Web;
using BlazorComponentHeap.GlobalEvents.Models;

namespace BlazorComponentHeap.GlobalEvents.Events;

public class BchMouseOverEventArgs : MouseEventArgs
{
    public List<ElementParameters> PathCoordinates { get; set; } = new();
}