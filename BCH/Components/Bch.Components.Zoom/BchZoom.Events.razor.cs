using Microsoft.AspNetCore.Components.Web;
using Bch.Modules.GlobalEvents.Events;
using Bch.Modules.Maths.Models;

namespace Bch.Components.Zoom;

public partial class BchZoom
{
    private bool _dragStarted = false;
    private int _touchMode = 0;
    private Vec2 _pinchPos = new();
    private float _touchStartDistance = 0;
    
    private readonly Vec2 _zoomTarget = new();
    private readonly Vec2 _zoomPoint = new();
    private readonly Vec2 _lastMousePosition = new();
    
    private Vec2 _prevRotateVec = new();
    private Vec2 _rotateVec = new();
    private Vec2 _touchDirToPos = new();
    private Vec2 _rotatedPoint = new();
    private float _rotationAngle = 0;
    private bool _userInteraction = false;
    
    private readonly Vec2 _pos = new();
    
    private Task OnMouseWheelAsync(BchWheelEventArgs e)
    {
        if (!ZoomOnMouseWheel) return Task.CompletedTask;

        var wrapper = e.PathCoordinates.FirstOrDefault(x => x.Id == _wrapperId);
        if (wrapper is null) return Task.CompletedTask;

        var container = e.PathCoordinates.FirstOrDefault(x => x.ClassList.Contains("navigation-cs-container"));
        if (container == null) return Task.CompletedTask;

        Transform(container.X, container.Y, -(float)e.DeltaY, 0, UseTouchRotation);

        return Task.CompletedTask;
    }
    
    private void OnMouseDown(MouseEventArgs e)
    {
        _lastMousePosition.Set(e.PageX, e.PageY);
        _dragStarted = true;
        _userInteraction = true;
        
        InvokeUpdateCallback();
        StateHasChanged();
    }

    private void OnMouseLeaveUp()
    {
        _dragStarted = false;
        _touchMode = 0;
        _prevRotateVec.Set(0, 0);
        _userInteraction = false;

        InvokeUpdateCallback();
        StateHasChanged();
    }

    private void OnMouseMove(MouseEventArgs e)
    {
        if (!_dragStarted) return;

        var mousePosition = new Vec2(e.PageX, e.PageY);
        var change = new Vec2(
            mousePosition.X - _lastMousePosition.X,
            mousePosition.Y - _lastMousePosition.Y
        );

        _lastMousePosition.Set(mousePosition);
        _pos.Add(change);
        _changePerformed = true;

        Update();
    }

    private async Task OnTouchStartAsync(TouchEventArgs e)
    {
        if (e.Touches.Length == 1)
        {
            _touchMode = 1;
            OnMouseDown(new MouseEventArgs
            {
                PageX = e.Touches[0].PageX,
                PageY = e.Touches[0].PageY
            });

            _userInteraction = true;
            InvokeUpdateCallback();

            return;
        }

        if (e.Touches.Length == 2)
        {
            _touchMode = 2;

            var dx = e.Touches[0].ScreenX - e.Touches[1].ScreenX;
            var dy = e.Touches[0].ScreenY - e.Touches[1].ScreenY;

            var wrapperRect = await DomInteropService.GetBoundingClientRectAsync(_wrapperId);
            if (wrapperRect is null) return;

            var x0 = e.Touches[0].PageX - wrapperRect.Left;
            var y0 = e.Touches[0].PageY - wrapperRect.Top;

            var x1 = e.Touches[1].PageX - wrapperRect.Left;
            var y1 = e.Touches[1].PageY - wrapperRect.Top;

            _pinchPos.Set(
                x0 + (x1 - x0) * 0.5f,
                y0 + (y1 - y0) * 0.5f);

            _touchStartDistance = (float)Math.Sqrt(dx * dx + dy * dy);
            _userInteraction = true;
            InvokeUpdateCallback();
        }
    }

    private void OnTouchMove(TouchEventArgs e)
    {
        if (e.Touches.Length == 1 && _touchMode == 1)
        {
            OnMouseMove(new MouseEventArgs
            {
                PageX = e.Touches[0].PageX,
                PageY = e.Touches[0].PageY
            });

            return;
        }

        if (e.Touches.Length == 2 && _touchMode == 2)
        {
            var dx = e.Touches[0].ScreenX - e.Touches[1].ScreenX;
            var dy = e.Touches[0].ScreenY - e.Touches[1].ScreenY;

            var touchDist2 = (float)Math.Sqrt((dx * dx) + (dy * dy));
            var deltaScale = touchDist2 - _touchStartDistance;

            _touchStartDistance = touchDist2;

            _rotateVec.Set(dx, dy);

            var deltaAngle = 0f;

            if (_prevRotateVec is not { X: 0, Y: 0 })
            {
                var dot = Vec2.DotProduct(_rotateVec, _prevRotateVec);
                var absA = _rotateVec.Length();
                var absB = _prevRotateVec.Length();

                var cos = dot / (absA * absB);
                var angleRadians = Math.Acos(cos);

                var cross = Vec2.CrossProduct(_rotateVec, _prevRotateVec);

                deltaAngle = (float)((cross < 0 ? 1 : -1) * angleRadians);

                if (double.IsNaN(deltaAngle)) deltaAngle = 0;
            }

            _prevRotateVec.Set(_rotateVec);
            Transform(_pinchPos.X, _pinchPos.Y, deltaScale / _dppx, deltaAngle, UseTouchRotation);

            return;
        }

        _touchMode = 0;
        _prevRotateVec.Set(0, 0);
    }

    private void OnTouchEnd(TouchEventArgs e)
    {
        OnMouseLeaveUp();
    }
}