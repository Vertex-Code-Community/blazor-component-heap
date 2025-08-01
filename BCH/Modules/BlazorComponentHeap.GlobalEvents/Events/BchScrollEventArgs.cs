using BlazorComponentHeap.GlobalEvents.Models;

namespace BlazorComponentHeap.GlobalEvents.Events;

public class BchScrollEventArgs : EventArgs
{
    public int ScrollTop { get; set; }
    public int ClientWidth { get; set; }
    public int ClientHeight { get; set; }
    public int ScrollHeight { get; set; }
    public List<ElementParameters> PathCoordinates { get; set; } = new();
}
