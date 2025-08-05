using Bch.Modules.Maths.Models;

namespace Bch.Components.Cropper.Models;

public class CropAreaModel
{
    public Vec2 Pos { get; set; } = new();
    public Vec2 Size { get; set; } = new();
}