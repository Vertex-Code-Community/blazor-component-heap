namespace Bch.Modules.GlobalEvents.Models;

public class BchTouchPoint
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
    
    public List<ElementParameters> PathCoordinates { get; set; } = new();
}