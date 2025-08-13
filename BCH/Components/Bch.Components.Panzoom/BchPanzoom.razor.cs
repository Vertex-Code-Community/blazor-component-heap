using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Bch.Components.Panzoom.Models;
using Microsoft.JSInterop;
using Bch.Modules.GlobalEvents.Services;
using Bch.Modules.GlobalEvents.Events;

namespace Bch.Components.Panzoom;

public partial class BchPanzoom : ComponentBase
{
    [Inject] private IJSRuntime Js { get; set; } = default!;
    [Inject] public required IGlobalEventsService GlobalEventsService { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public PanzoomOptions Options { get; set; } = new();

    private string _containerId = $"_pz_cnt_{Guid.NewGuid()}";
    private string _contentId = $"_pz_c_{Guid.NewGuid()}";
    private ElementReference _containerRef;

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

    protected override Task OnInitializedAsync()
    {
        _scale = Options.InitialScale;
        _x = Options.InitialX;
        _y = Options.InitialY;

        // Subscribe here (like BchSelect), not in OnAfterRender
        // No global subscription needed when using custom element event
        return Task.CompletedTask;
    }

    private void OnScrollCustom(BchWheelEventArgs e)
    {
        var delta = e.DeltaY;
        var wheel = (float)Math.Exp(-Options.ZoomWheelSpeed * delta);
        var newScale = Clamp(_scale * wheel, Options.MinScale, Options.MaxScale);
        ZoomToPoint(e.X, e.Y, newScale);
    }
    
    private string GetTransformStyle()
    {
        var nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
        return $"transform: translate({_x.ToString(nfi)}px, {_y.ToString(nfi)}px) scale({_scale.ToString(nfi)}); transform-origin: {_lastLocalX.ToString(nfi)}px {_lastLocalY.ToString(nfi)}px;";
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
        _lastLocalX = (float)e.OffsetX;
        _lastLocalY = (float)e.OffsetY;
        Console.WriteLine($"[BchPanzoom] DoubleClick at offset=({e.OffsetX}, {e.OffsetY}) scale {_scale} -> {newScale} scale={_scale}");
        ZoomToPoint(_lastLocalX, _lastLocalY, newScale);
    }

    private void OnWheel(WheelEventArgs e)
    {
        if (!Options.EnableWheel || Options.DisableZoom) return;
        var delta = e.DeltaY;
        var wheel = (float)Math.Exp(-Options.ZoomWheelSpeed * delta);
        var newScale = Clamp(_scale * wheel, Options.MinScale, Options.MaxScale);
        Console.WriteLine($"[BchPanzoom] Wheel deltaY={delta}, scale {_scale} -> {newScale} at offset=({e.OffsetX}, {e.OffsetY}) scale={_scale}");
        _lastLocalX = (float)e.OffsetX;
        _lastLocalY = (float)e.OffsetY;
        ZoomToPoint(_lastLocalX, _lastLocalY, newScale);
    }

    private void OnTouchStartCustom(BchTouchEventArgs e)
    {
        Console.WriteLine($"[BchPanzoom] TouchStart touches={e.Touches.Count} scale={_scale}");
        if (!Options.EnableTouch) return;
        if (e.Touches.Count == 1)
        {
            if (Options.DisablePan || (Options.PanOnlyWhenZoomed && IsApproximatelyOne(_scale))) return;
            var t = e.Touches[0];
            _isPanning = true;
            _startX = t.ClientX;
            _startY = t.ClientY;
            _startPanX = _x;
            _startPanY = _y;
        }
        else if (e.Touches.Count >= 2 && !Options.DisableZoom)
        {
            var dx = e.Touches[1].ClientX - e.Touches[0].ClientX;
            var dy = e.Touches[1].ClientY - e.Touches[0].ClientY;
            _pinchStartDistance = MathF.Sqrt(dx * dx + dy * dy);
            _pinchStartScale = _scale;
            _isPinching = true;
            Console.WriteLine($"[BchPanzoom] Pinch start distance={_pinchStartDistance}, scale={_pinchStartScale} scale={_scale}");
        }
    }

    private void OnTouchMoveCustom(BchTouchEventArgs e)
    {
        if (!Options.EnableTouch) return;
        if (_isPinching && e.Touches.Count >= 2)
        {
            var dx = e.Touches[1].ClientX - e.Touches[0].ClientX;
            var dy = e.Touches[1].ClientY - e.Touches[0].ClientY;
            var dist = MathF.Sqrt(dx * dx + dy * dy);
            var scaleFactor = dist / _pinchStartDistance;
            var newScale = Clamp(_pinchStartScale * scaleFactor, Options.MinScale, Options.MaxScale);
            var centerLocalX = (e.Touches[0].X + e.Touches[1].X) / 2.0f;
            var centerLocalY = (e.Touches[0].Y + e.Touches[1].Y) / 2.0f;
            _lastLocalX = centerLocalX;
            _lastLocalY = centerLocalY;
            ZoomToPoint(_lastLocalX, _lastLocalY, newScale);
            Console.WriteLine($"[BchPanzoom] Pinch move distance={dist}, newScale={newScale}");
            StateHasChanged();
            return;
        }
        if (_isPanning && e.Touches.Count == 1)
        {
            var t = e.Touches[0];
            var dx = t.ClientX - _startX;
            var dy = t.ClientY - _startY;
            _x = _startPanX + dx;
            _y = _startPanY + dy;
            Console.WriteLine($"[BchPanzoom] Touch pan dx={dx}, dy={dy}, pos=({_x}, {_y})");
            StateHasChanged();
        }
    }

    private void OnTouchEndCustom(BchTouchEventArgs e)
    {
        Console.WriteLine($"[BchPanzoom] TouchEnd touches={e.Touches.Count} panning={_isPanning} pinching={_isPinching}");
        _isPanning = false;
        _isPinching = false;
    }

    private void ZoomToPoint(float localX, float localY, float newScale)
    {
        var oldScale = _scale;
        if (Math.Abs(newScale - oldScale) < 0.0001f) return;
        var contentX = (localX - _x) / oldScale;
        var contentY = (localY - _y) / oldScale;

        _scale = newScale;
        // Keep the zoom target anchored under the cursor
        _x = localX - contentX * _scale;
        _y = localY - contentY * _scale;
        Console.WriteLine($"[BchPanzoom] ZoomToPoint ({localX}, {localY}) oldScale={oldScale} newScale={newScale} pos=({_x}, {_y})");
        StateHasChanged();
    }

    private float _lastLocalX;
    private float _lastLocalY;

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


