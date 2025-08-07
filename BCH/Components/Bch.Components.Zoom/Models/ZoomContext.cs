using Bch.Modules.Maths.Models;

namespace Bch.Components.Zoom.Models;

public class ZoomContext
{
    public Vec2 TopLeftPos { get; } = new();
    public float Scale { get; set; } = 1.0f;
    public float ScaleLinear { get; set; } = 1.0f;
    public float AngleInRadians { get; set; } = 1.0f;
    public Action? ZoomUp { get; set; }
    public Func<float, Task>? ZoomDown { get; set; }
    public Func<Task>? OnUpdate { get; set; }
    public bool UserInteraction { get; set; }
}