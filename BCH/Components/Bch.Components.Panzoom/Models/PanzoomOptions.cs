namespace Bch.Components.Panzoom.Models;

public class PanzoomOptions
{
    public float MinScale { get; set; } = 0.25f;
    public float MaxScale { get; set; } = 8.0f;
    public float Step { get; set; } = 0.2f;
    public float InitialScale { get; set; } = 1.0f;
    public float InitialX { get; set; } = 0.0f;
    public float InitialY { get; set; } = 0.0f;

    public bool DisablePan { get; set; } = false;
    public bool DisableZoom { get; set; } = false;
    public bool PanOnlyWhenZoomed { get; set; } = false;

    public bool Contain { get; set; } = true;
    public float BoundsPadding { get; set; } = 0.0f;

    public bool EnableWheel { get; set; } = true;
    public bool EnableDoubleClick { get; set; } = true;
    public bool EnableTouch { get; set; } = true;

    public float ZoomWheelSpeed { get; set; } = 0.065f;
    public float ZoomDoubleClickSpeed { get; set; } = 1.0f;
}


