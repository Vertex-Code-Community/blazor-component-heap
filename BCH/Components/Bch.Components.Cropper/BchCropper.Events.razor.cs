using Bch.Components.Cropper.Models;
using Bch.Modules.GlobalEvents.Events;
using Microsoft.AspNetCore.Components.Web;

namespace Bch.Components.Cropper;

public partial class BchCropper
{
    private Task OnMouseWheel(BchWheelEventArgs e)
    {
        if (!ScaleOnMouseWheel) return Task.CompletedTask;

        var movableId = CropperType == CropperType.Circle ? _circleId : _rectId;
        
        var rectWrapper = e.PathCoordinates.FirstOrDefault(x => x.Id == movableId);
        if (rectWrapper is null) return Task.CompletedTask;
        
        var wrapper = e.PathCoordinates.FirstOrDefault(x => x.Id == _cropperId);
        if (wrapper is null) return Task.CompletedTask;

        _bchZoom?.Transform(wrapper.X, wrapper.Y, -(float)e.DeltaY, 0);
        return Task.CompletedTask;
    }
    
    private Task OnTouchStartAsync(TouchEventArgs args, bool rectDragged)
    {
        return OnMouseDownAsync(new MouseEventArgs
        {
            PageX = args.Touches[0].PageX,
            PageY = args.Touches[0].PageY
        }, rectDragged);
    }

    private async Task OnMouseDownAsync(MouseEventArgs e, bool rectDragged)
    {
        await GlobalEventsService.AddDocumentListenerAsync<MouseEventArgs>("mouseup", _key, OnMouseLeaveUpAsync);
        await GlobalEventsService.AddDocumentListenerAsync<MouseEventArgs>("touchend", _key, OnMouseLeaveUpAsync);
        await GlobalEventsService.AddDocumentListenerAsync<MouseEventArgs>("mousemove", _key, OnMouseMoveAsync);
        await GlobalEventsService.AddDocumentListenerAsync<TouchEventArgs>("touchmove", _key, OnTouchMoveAsync);

        _lastMousePosition.Set(e.PageX, e.PageY);
        _rectDragged = rectDragged;
        _rectHandleDragged = !rectDragged;

        StateHasChanged();
    }

    private async Task OnMouseLeaveUpAsync(MouseEventArgs _)
    {
        await GlobalEventsService.RemoveDocumentListenerAsync<MouseEventArgs>("mouseup", _key);
        await GlobalEventsService.RemoveDocumentListenerAsync<MouseEventArgs>("touchend", _key);
        await GlobalEventsService.RemoveDocumentListenerAsync<MouseEventArgs>("mousemove", _key);
        await GlobalEventsService.RemoveDocumentListenerAsync<TouchEventArgs>("touchmove", _key);

        if (!_rectDragged && !_rectHandleDragged) return;

        _rectDragged = false;
        _rectHandleDragged = false;

        if (_rectSize.X < MinRectangleWidth) _rectSize.X = MinRectangleWidth;
        if (_rectSize.Y < MinRectangleHeight) _rectSize.Y = MinRectangleHeight;

        StateHasChanged();
    }

    private Task OnTouchMoveAsync(TouchEventArgs args)
    {
        if (args.Touches.Length != 1) return OnMouseLeaveUpAsync(new MouseEventArgs());

        _eventObj.PageX = args.Touches[0].PageX;
        _eventObj.PageY = args.Touches[0].PageY;

        return OnMouseMoveAsync(_eventObj);
    }

    private Task OnMouseMoveAsync(MouseEventArgs e)
    {
        if (!_rectDragged && !_rectHandleDragged) return Task.CompletedTask;

        _change.Set(
            e.PageX - _lastMousePosition.X,
            e.PageY - _lastMousePosition.Y
        );

        _lastMousePosition.Set(e.PageX, e.PageY);

        if (_change is { X: 0, Y: 0 }) return Task.CompletedTask;

        var valueToChange = _rectDragged ? _rectPos : _rectSize;
        valueToChange.Add(_change);

        StateHasChanged();

        return Task.CompletedTask;
    }
}