using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Bch.Components.Panzoom.Models;

namespace Bch.Components.Panzoom;

public partial class BchPanzoom : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public PanzoomOptions Options { get; set; } = new();

    private string _containerId = $"_pz_cnt_{Guid.NewGuid()}";
    private string _contentId = $"_pz_c_{Guid.NewGuid()}";

    private float _scale;
    private float _x;
    private float _y;

    private bool _isPanning = false;
    private float _startX;
    private float _startY;
    private float _startPanX;
    private float _startPanY;

    private bool _isPinching = false;
    private float _pinchStartDistance;
    private float _pinchStartScale;

    protected override void OnInitialized()
    {
        _scale = Options.InitialScale;
        _x = Options.InitialX;
        _y = Options.InitialY;
    }

    private string GetTransformStyle()
    {
        var nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
        return $"transform: translate({_x.ToString(nfi)}px, {_y.ToString(nfi)}px) scale({_scale.ToString(nfi)}); transform-origin: 0 0;";
    }

    private void OnMouseDown(MouseEventArgs e)
    {
        Console.WriteLine($"[BchPanzoom] MouseDown at ({e.ClientX}, {e.ClientY}) scale={_scale}");
        if (Options.DisablePan) return;
        // Allow pan when zoomed out (< 1). Only block when exactly at 1x if PanOnlyWhenZoomed is true
        if (Options.PanOnlyWhenZoomed && IsApproximatelyOne(_scale)) return;
        _isPanning = true;
        _startX = (float)e.ClientX;
        _startY = (float)e.ClientY;
        _startPanX = _x;
        _startPanY = _y;
    }

    private void OnMouseUp(MouseEventArgs e)
    {
        Console.WriteLine($"[BchPanzoom] MouseUp at ({e.ClientX}, {e.ClientY}) panningWas={_isPanning} scale={_scale}");
        _isPanning = false;
    }

    private void OnMouseMove(MouseEventArgs e)
    {
        if (!_isPanning || Options.DisablePan) return;
        var dx = (float)e.ClientX - _startX;
        var dy = (float)e.ClientY - _startY;
        _x = _startPanX + dx;
        _y = _startPanY + dy;
        Console.WriteLine($"[BchPanzoom] MouseMove dx={dx}, dy={dy}, pos=({_x}, {_y}) scale={_scale}");
        StateHasChanged();
    }

    private void OnDoubleClick(MouseEventArgs e)
    {
        if (!Options.EnableDoubleClick || Options.DisableZoom) return;
        var newScale = Clamp(_scale + Options.Step * MathF.Max(1f, Options.ZoomDoubleClickSpeed), Options.MinScale, Options.MaxScale);
        Console.WriteLine($"[BchPanzoom] DoubleClick at offset=({e.OffsetX}, {e.OffsetY}) scale {_scale} -> {newScale} scale={_scale}");
        ZoomToPoint((float)e.OffsetX, (float)e.OffsetY, newScale);
    }

    private void OnWheel(WheelEventArgs e)
    {
        if (!Options.EnableWheel || Options.DisableZoom) return;
        var delta = e.DeltaY;
        var wheel = (float)Math.Exp(-Options.ZoomWheelSpeed * delta);
        var newScale = Clamp(_scale * wheel, Options.MinScale, Options.MaxScale);
        Console.WriteLine($"[BchPanzoom] Wheel deltaY={delta}, scale {_scale} -> {newScale} at offset=({e.OffsetX}, {e.OffsetY}) scale={_scale}");
        ZoomToPoint((float)e.OffsetX, (float)e.OffsetY, newScale);
    }

    private void OnTouchStart(TouchEventArgs e)
    {
        Console.WriteLine($"[BchPanzoom] TouchStart touches={e.Touches.Length} scale={_scale}");
        if (!Options.EnableTouch) return;
        if (e.Touches.Length == 1)
        {
            if (Options.DisablePan || (Options.PanOnlyWhenZoomed && IsApproximatelyOne(_scale))) return;
            var t = e.Touches[0];
            _isPanning = true;
            _startX = (float)t.ClientX;
            _startY = (float)t.ClientY;
            _startPanX = _x;
            _startPanY = _y;
        }
        else if (e.Touches.Length >= 2 && !Options.DisableZoom)
        {
            var dx = (float)(e.Touches[1].ClientX - e.Touches[0].ClientX);
            var dy = (float)(e.Touches[1].ClientY - e.Touches[0].ClientY);
            _pinchStartDistance = MathF.Sqrt(dx * dx + dy * dy);
            _pinchStartScale = _scale;
            _isPinching = true;
            Console.WriteLine($"[BchPanzoom] Pinch start distance={_pinchStartDistance}, scale={_pinchStartScale} scale={_scale}");
        }
    }

    private void OnTouchMove(TouchEventArgs e)
    {
        if (!Options.EnableTouch) return;
        if (_isPinching && e.Touches.Length >= 2)
        {
            var dx = (float)(e.Touches[1].ClientX - e.Touches[0].ClientX);
            var dy = (float)(e.Touches[1].ClientY - e.Touches[0].ClientY);
            var dist = MathF.Sqrt(dx * dx + dy * dy);
            var scaleFactor = dist / _pinchStartDistance;
            var newScale = Clamp(_pinchStartScale * scaleFactor, Options.MinScale, Options.MaxScale);
            _scale = newScale;
            Console.WriteLine($"[BchPanzoom] Pinch move distance={dist}, scale={_scale}");
            StateHasChanged();
            return;
        }
        if (_isPanning && e.Touches.Length == 1)
        {
            var t = e.Touches[0];
            var dx = (float)t.ClientX - _startX;
            var dy = (float)t.ClientY - _startY;
            _x = _startPanX + dx;
            _y = _startPanY + dy;
            Console.WriteLine($"[BchPanzoom] Touch pan dx={dx}, dy={dy}, pos=({_x}, {_y})");
            StateHasChanged();
        }
    }

    private void OnTouchEnd(TouchEventArgs e)
    {
        Console.WriteLine($"[BchPanzoom] TouchEnd touches={e.Touches.Length} panning={_isPanning} pinching={_isPinching}");
        _isPanning = false;
        _isPinching = false;
    }

    private void ZoomToPoint(float clientX, float clientY, float newScale)
    {
        var oldScale = _scale;
        if (Math.Abs(newScale - oldScale) < 0.0001f) return;
        var contentX = (clientX - _x) / oldScale;
        var contentY = (clientY - _y) / oldScale;
        _scale = newScale;
        _x = clientX - contentX * _scale;
        _y = clientY - contentY * _scale;
        Console.WriteLine($"[BchPanzoom] ZoomToPoint ({clientX}, {clientY}) oldScale={oldScale} newScale={newScale} pos=({_x}, {_y})");
        StateHasChanged();
    }

    private static float Clamp(float v, float min, float max)
    {
        if (v < min) return min;
        if (v > max) return max;
        return v;
    }

    private static bool IsApproximatelyOne(float value)
    {
        return MathF.Abs(value - 1.0f) <= 0.0001f;
    }
}


