using BlazorComponentHeap.Shared.Models.Math;

namespace BlazorComponentHeap.Components.Models.Cropper;

public class CropAreaModel
{
    public Vec2 Pos { get; set; } = new();
    public Vec2 Size { get; set; } = new();
}