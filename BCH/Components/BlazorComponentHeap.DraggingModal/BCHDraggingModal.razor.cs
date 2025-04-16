using System.Globalization;
using BlazorComponentHeap.Core.Models.Events;
using BlazorComponentHeap.Core.Models.Math;
using BlazorComponentHeap.Core.Services.Interfaces;
using BlazorComponentHeap.Modal.Modal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.DraggingModal;

public partial class BCHDraggingModal : IAsyncDisposable
{
    private static int _globalZIndex = 999999;

    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] public IJSUtilsService JsUtilsService { get; set; } = null!;
    [Parameter] public string ContainerId { get; set; } = $"_id_{Guid.NewGuid()}";
    [Parameter] public string HandleId { get; set; } = $"_id_{Guid.NewGuid()}";
    [Parameter] public string Width { get; set; } = "100px";
    [Parameter] public string Height { get; set; } = "100px";
    [Parameter] public int InitialX { get; set; }
    [Parameter] public int InitialY { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    // [Parameter] public EventCallback<ExtMouseEventArgs> OnMouseDown { get; set; }
    // [Parameter] public EventCallback<ExtMouseEventArgs> OnMouseMove { get; set; }
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
    private BCHModal? _modalComponent = null!;

    private readonly string _key = $"_id_{Guid.NewGuid()}";
    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };
    private readonly Vec2 _lastMousePosition = new();
    private readonly Vec2 _pos = new();
    private readonly Vec2 _offset = new();

    protected override async Task OnInitializedAsync()
    {
        _pos.Set(InitialX, InitialY);
        
        await JsUtilsService.AddDocumentListenerAsync<ExtMouseEventArgs>("mousedown", _key, OnDocumentMouseDown);
        await JsUtilsService.AddDocumentListenerAsync<ExtMouseEventArgs>("mousemove", _key, OnDocumentMouseMoveAsync);
        await JsUtilsService.AddDocumentListenerAsync<ExtTouchEventArgs>("touchmove", _key, OnDocumentTouchMove);
        await JsUtilsService.AddDocumentListenerAsync<ExtTouchEventArgs>("touchstart", _key, OnDocumentTouchStartAsync);
        await JsUtilsService.AddDocumentListenerAsync<ExtTouchEventArgs>("touchend", _key, OnDocumentTouchEndAsync);
        await JsUtilsService.AddDocumentListenerAsync<object>("mouseup", _key, OnMouseLeaveUp);
        await JsUtilsService.AddDocumentListenerAsync<object>("contextmenu", _key, OnMouseLeaveUp);
    }

    public async ValueTask DisposeAsync()
    {
        await JsUtilsService.RemoveDocumentListenerAsync<ExtMouseEventArgs>("mousedown", _key);
        await JsUtilsService.RemoveDocumentListenerAsync<ExtMouseEventArgs>("mousemove", _key);
        await JsUtilsService.RemoveDocumentListenerAsync<ExtTouchEventArgs>("touchmove", _key);
        await JsUtilsService.RemoveDocumentListenerAsync<ExtTouchEventArgs>("touchstart", _key);
        await JsUtilsService.RemoveDocumentListenerAsync<ExtTouchEventArgs>("touchend", _key);
        await JsUtilsService.RemoveDocumentListenerAsync<object>("mouseup", _key);
        await JsUtilsService.RemoveDocumentListenerAsync<object>("contextmenu", _key);
    }

    private Task OnDocumentMouseDown(ExtMouseEventArgs e)
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

    private async Task OnDocumentMouseMoveAsync(ExtMouseEventArgs e)
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

    private Task OnDocumentTouchMove(ExtTouchEventArgs e)
    {
        if (e.Touches.Count != 1) return Task.CompletedTask;
        var touch = e.Touches.First();
        
        return OnDocumentMouseMoveAsync(new ExtMouseEventArgs()
        {
            PageX = touch.PageX,
            PageY = touch.PageY,
            PathCoordinates = touch.PathCoordinates
        });
    }
    
    private Task OnDocumentTouchEndAsync(ExtTouchEventArgs e)
    {
        return OnMouseLeaveUp(new object());
    }
    
    private Task OnDocumentTouchStartAsync(ExtTouchEventArgs e)
    {
        if (e.Touches.Count != 1) return Task.CompletedTask;
        var touch = e.Touches.First();
        
        return OnDocumentMouseDown(new ExtMouseEventArgs
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