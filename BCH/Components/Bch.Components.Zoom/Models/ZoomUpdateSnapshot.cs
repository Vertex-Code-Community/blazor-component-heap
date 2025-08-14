namespace Bch.Components.Zoom.Models;

public struct ZoomUpdateSnapshot
{
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float Scale { get; set; }
    public float ScaleLinear { get; set; }
    public float AngleInRadians { get; set; }
    public bool UserInteracttion { get; set; }
}