namespace BlazorComponentHeap.Core.Models.Markup;

public class ExtTouchPoint
{
    public float X { get; set; }
    public float Y { get; set; }
    
    public float ScreenX { get; set; }
    public float ScreenY { get; set; }
    public float ClientWidth { get; set; }
    public float ClientHeight { get; set; }
    public float ClientX { get; set; }
    public float ClientY { get; set; }
    public float PageX { get; set; }
    public float PageY { get; set; }
    
    public List<CoordsHolder> PathCoordinates { get; set; } = new();
}