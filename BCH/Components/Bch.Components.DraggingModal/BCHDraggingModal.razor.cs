using System.Globalization;
using Microsoft.AspNetCore.Components;
using Bch.Modules.GlobalEvents.Events;
using Bch.Modules.GlobalEvents.Services;
using Bch.Modules.Maths.Models;
using Bch.Components.Modal;

namespace Bch.Components.DraggingModal;

public partial class BchDraggingModal : IAsyncDisposable
{
    private static int _globalZIndex = 999999;

    [Inject] public required IGlobalEventsService GlobalEventsService { get; set; }
    
    [Parameter] public string ContainerId { get; set; } = $"_id_{Guid.NewGuid()}";
    [Parameter] public string HandleId { get; set; } = $"_id_{Guid.NewGuid()}";
    [Parameter] public string Width { get; set; } = "100px";
    [Parameter] public string Height { get; set; } = "100px";
    [Parameter] public int InitialX { get; set; }
    [Parameter] public int InitialY { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public EventCallback<bool> ShowChanged { get; set; }
    [Parameter] public bool Show
    {
        get => _show;
        set
        {
            if (_show == value) return;
            _show = value;

            ShowChanged.InvokeAsync(value);
        }
    }

    private bool _show { get; set; }
    private bool _dragStarted = false;
    private int _zIndex = _globalZIndex;
    private BchModal? _modalComponent = null!;

    private readonly string _key = $"_id_{Guid.NewGuid()}";
    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };
    private readonly Vec2 _lastMousePosition = new();
    private readonly Vec2 _pos = new();
    private readonly Vec2 _offset = new();

    protected override async Task OnInitializedAsync()
    {
        _pos.Set(InitialX, InitialY);
        
        await GlobalEventsService.AddDocumentListenerAsync<BchMouseEventArgs>("mousedown", _key, OnDocumentMouseDown);
        await GlobalEventsService.AddDocumentListenerAsync<BchMouseEventArgs>("mousemove", _key, OnDocumentMouseMoveAsync);
        await GlobalEventsService.AddDocumentListenerAsync<BchTouchEventArgs>("touchmove", _key, OnDocumentTouchMove);
        await GlobalEventsService.AddDocumentListenerAsync<BchTouchEventArgs>("touchstart", _key, OnDocumentTouchStartAsync);
        await GlobalEventsService.AddDocumentListenerAsync<BchTouchEventArgs>("touchend", _key, OnDocumentTouchEndAsync);
        await GlobalEventsService.AddDocumentListenerAsync<object>("mouseup", _key, OnMouseLeaveUp);
        await GlobalEventsService.AddDocumentListenerAsync<object>("contextmenu", _key, OnMouseLeaveUp);
    }

    public async ValueTask DisposeAsync()
    {
        await GlobalEventsService.RemoveDocumentListenerAsync<BchMouseEventArgs>("mousedown", _key);
        await GlobalEventsService.RemoveDocumentListenerAsync<BchMouseEventArgs>("mousemove", _key);
        await GlobalEventsService.RemoveDocumentListenerAsync<BchTouchEventArgs>("touchmove", _key);
        await GlobalEventsService.RemoveDocumentListenerAsync<BchTouchEventArgs>("touchstart", _key);
        await GlobalEventsService.RemoveDocumentListenerAsync<BchTouchEventArgs>("touchend", _key);
        await GlobalEventsService.RemoveDocumentListenerAsync<object>("mouseup", _key);
        await GlobalEventsService.RemoveDocumentListenerAsync<object>("contextmenu", _key);
    }

    private Task OnDocumentMouseDown(BchMouseEventArgs e)
    {
        if (e.PathCoordinates.Any(x => x.ClassList.Contains("close-btn") || x.ClassList.Contains("icon"))) 
            return Task.CompletedTask;
        var coordsHolder = e.PathCoordinates.FirstOrDefault();

        if (e.PathCoordinates.FirstOrDefault(x => x.Id == ContainerId) != null)
        {
            // await OnMouseDown.InvokeAsync(e);
            _zIndex = ++_globalZIndex;
            if (coordsHolder?.Id != HandleId) StateHasChanged();
        }

        if (coordsHolder?.Id != HandleId) return Task.CompletedTask;

        _lastMousePosition.Set(e.PageX, e.PageY);
        _dragStarted = true;

        StateHasChanged();
        
        return Task.CompletedTask;
    }

    private async Task OnDocumentMouseMoveAsync(BchMouseEventArgs e)
    {
        if (!_dragStarted) return;

        var mousePosition = new Vec2(e.PageX, e.PageY);
        var change = new Vec2(
            mousePosition.X - _lastMousePosition.X,
            mousePosition.Y - _lastMousePosition.Y
        );

        _lastMousePosition.Set(mousePosition);
        _pos.Add(change);

        // await OnMouseMove.InvokeAsync(e);
        await _modalComponent!.SetPositionAsync($"{_pos.X.ToString(_nF)}px", $"{_pos.Y.ToString(_nF)}px");
    }

    private Task OnDocumentTouchMove(BchTouchEventArgs e)
    {
        if (e.Touches.Count != 1) return Task.CompletedTask;
        var touch = e.Touches.First();
        
        return OnDocumentMouseMoveAsync(new BchMouseEventArgs
        {
            PageX = touch.PageX,
            PageY = touch.PageY,
            PathCoordinates = touch.PathCoordinates
        });
    }
    
    private Task OnDocumentTouchEndAsync(BchTouchEventArgs e)
    {
        return OnMouseLeaveUp(new object());
    }
    
    private Task OnDocumentTouchStartAsync(BchTouchEventArgs e)
    {
        if (e.Touches.Count != 1) return Task.CompletedTask;
        var touch = e.Touches.First();
        
        return OnDocumentMouseDown(new BchMouseEventArgs
        {
            PageX = touch.PageX,
            PageY = touch.PageY,
            PathCoordinates = touch.PathCoordinates
        });
    }
    
    private Task OnMouseLeaveUp(object _)
    {
        _dragStarted = false;
        StateHasChanged();

        return Task.CompletedTask;
    }

    public async Task SetPositionAsync(float x, float y)
    {
        _pos.Set(x, y);
        await _modalComponent!.SetPositionAsync($"{_pos.X.ToString(_nF)}px", $"{_pos.Y.ToString(_nF)}px");
    }
}