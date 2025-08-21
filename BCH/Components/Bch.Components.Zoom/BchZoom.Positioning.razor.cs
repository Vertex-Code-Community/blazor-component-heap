using Microsoft.AspNetCore.Components;
using Bch.Components.Zoom.Models;
using Bch.Modules.Maths.Models;

namespace Bch.Components.Zoom;

public partial class BchZoom
{
    [Parameter] public InitialZoomPositioning InitialPositioning { get; set; } = InitialZoomPositioning.None;
    [Parameter] public float? InitialScale { get; set; }
    [Parameter] public float? InitialAngle { get; set; }
    [Parameter] public Vec2? InitialPosition { get; set; }

    private void ApplyInitialPositioning()
    {
        if (InitialPositioning == InitialZoomPositioning.None)
        {
            if (InitialScale is not null) _scale = (float)Math.Log(InitialScale.Value) + 4;
            if (InitialPosition is not null) _pos.Set(InitialPosition);
            if (InitialAngle is not null) _rotationAngle = InitialAngle.Value;

            return;
        }

        if (InitialPositioning == InitialZoomPositioning.FitOutsideCentered)
            ApplyOutsidePositioning();
        
        if (InitialPositioning == InitialZoomPositioning.FitInsideCentered)
            ApplyInsidePositioning();

        Update();
    }

    private void ApplyOutsidePositioning()
    {
        var navigationSizeX = ContentSize.X;
        var navigationSizeY = ContentSize.Y;
        
        var scale = _viewPortSize.X / navigationSizeX;
        var newHeight = navigationSizeY * scale;

        var x = 0.0f;
        var y = _viewPortSize.Y * 0.5f - newHeight * 0.5f;
        
        var heightCondition = newHeight < _viewPortSize.Y;
        
        if (heightCondition)
        {
            scale = _viewPortSize.Y / navigationSizeY;
            var newWidth = navigationSizeX * scale;
            
            x = _viewPortSize.X * 0.5f - newWidth * 0.5f;
            y = 0;
        }
        
        _pos.Set(x, y);
        _scale = (float) Math.Log(scale) + 4;
    }

    private void ApplyInsidePositioning()
    {
        
    }
}