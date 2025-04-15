using BlazorComponentHeap.Core.Services.Interfaces;
using BlazorComponentHeap.Shared.Models.Markup;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;

namespace BlazorComponentHeap.Components.Range;

public partial class BCHRange : IDisposable
{
    [Inject] private IJSUtilsService JsUtilsService { get; set; } = null!;

    [Parameter] public int Step { get; set; }
    [Parameter] public int AfterPointCountNumber { get; set; }
    [Parameter] public float Min { get; set; }
    [Parameter] public float Max { get; set; }
    [Parameter] public bool ShowTooltip { get; set; } = false;
    [Parameter] public bool Vertical { get; set; }
    [Parameter] public bool ShowFillColor { get; set; }
    [Parameter] public string TypeValue { get; set; } = string.Empty;
    [Parameter] public EventCallback<float> ValueChanged { get; set; }
    [Parameter] public float Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;

            var points = Max - Min;

            if (Vertical)
            {
                _offsetY = ((_value - Min) * ContainerHeight) / points;
            }
            else
            {
                _offsetX = ((_value - Min) * ContainerWidth) / points;
            }

            ValueChanged.InvokeAsync(value);
        }
    }

    private string _circleId = $"_id_{Guid.NewGuid()}";
    private string _containerId = $"_id_{Guid.NewGuid()}";

    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };

    private float _offsetX = 0;
    private float _offsetY = 0;
    private bool _mouseDownOnCircle;
    private bool _isMouse; // or touch otherwise
    private BoundingClientRect _thumb = new();
    private BoundingClientRect _container = new();
    private float _thumbHeight;
    private float _thumbWidth;
    private bool _showTooltip;
    private float _value;
    private int _stepInPixels;
    private bool _firstRender;
    private float _widthStep;
    private float _heightStep;
    private float ContainerWidth => _container.Width - _thumb.Width;

    private float ContainerHeight => _container.Height - _thumb.Height;

    private float _prevPageY;
    private float _prevPageX;

    protected override void OnInitialized()
    {
        IJSUtilsService.OnResize += OnResizeAsync;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _firstRender = firstRender;

        if (firstRender)
        {
            await OnUpdateAsync();

            var points = Max - Min;

            if (Vertical)
            {
                _offsetY = ((_value - Min) * ContainerHeight) / points;
            }
            else
            {
                _offsetX = ((_value - Min) * ContainerWidth) / points;
            }

        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_firstRender)
        {
            await OnUpdateAsync();
        }
    }

    #region Horizontal

    private async Task OnContainerHorizontalDownAsync(float pageX, bool isMouse)
    {
        if (_mouseDownOnCircle) return;

        _isMouse = isMouse;
        _showTooltip = true;
        _mouseDownOnCircle = true;

        if (isMouse)
        {
            await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("mouseup",  _containerId, OnHorizontalUpAsync);
            await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("mousemove",  _containerId, OnHorizontalMouseMoveAsync);
        }
        else
        {
            await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("touchend", _containerId, OnHorizontalUpAsync);
            await JsUtilsService.AddDocumentListenerAsync<TouchEventArgs>("touchmove",  _containerId, OnHorizontalTouchMoveAsync);
        }
        
        _thumb = await JsUtilsService.GetBoundingClientRectAsync(_circleId);
        
        _prevPageX = pageX;
        _offsetX = pageX - _container.Left - _thumb.Width * 0.5f;
        _offsetX = Math.Clamp(_offsetX, 0, _container.Width - _thumb.Width);

        UpdateValue();
    }

    private async Task OnHorizontalDownAsync(float pageX, bool isMouse)
    {
        _isMouse = isMouse;
        _showTooltip = true;
        _mouseDownOnCircle = true;
        _prevPageX = pageX;
        
        if (isMouse)
        {
            await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("mouseup",  _containerId, OnHorizontalUpAsync);
            await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("mousemove",  _containerId, OnHorizontalMouseMoveAsync);
        }
        else
        {
            await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("touchend", _containerId, OnHorizontalUpAsync);
            await JsUtilsService.AddDocumentListenerAsync<TouchEventArgs>("touchmove",  _containerId, OnHorizontalTouchMoveAsync);
        }

        _thumb = await JsUtilsService.GetBoundingClientRectAsync(_circleId);
    }

    [JSInvokable]
    public async Task OnHorizontalUpAsync(MouseEventArgs _)
    {
        if (_isMouse)
        {
            await JsUtilsService.RemoveDocumentListenerAsync<MouseEventArgs>("mouseup", _containerId);
            await JsUtilsService.RemoveDocumentListenerAsync<MouseEventArgs>("mousemove", _containerId);
        }
        else
        {
            await JsUtilsService.RemoveDocumentListenerAsync<MouseEventArgs>("touchend", _containerId);
            await JsUtilsService.RemoveDocumentListenerAsync<TouchEventArgs>("touchmove", _containerId);
        }
        
        _showTooltip = false;
        _mouseDownOnCircle = false;

        StateHasChanged();
    }

    [JSInvokable]
    public Task OnHorizontalMouseMoveAsync(MouseEventArgs mouseEvent)
    {
        OnHorizontalMove((float) mouseEvent.PageX);

        return Task.CompletedTask;
    }
    
    [JSInvokable]
    public async Task OnHorizontalTouchMoveAsync(TouchEventArgs touchEvent)
    {
        if (!_mouseDownOnCircle) return;

        if (touchEvent.Touches.Length == 1)
        {
            OnHorizontalMove((float) touchEvent.Touches[0].PageX);
            return;
        }

        await OnHorizontalUpAsync(new MouseEventArgs());
    }

    private void OnHorizontalMove(float pageX)
    {
        if (!_mouseDownOnCircle) return;
        
        var dX = pageX - _prevPageX;
        
        _prevPageX = pageX;
        _offsetX += dX;
        _offsetX = Math.Clamp(_offsetX, 0, _container.Width - _thumb.Width);

        UpdateValue();
    }

    private string GetLeftOffset()
    {
        return (((_value - Min) / (Max - Min)) * ContainerWidth).ToString(_nF);
    }

    private string GetFillOffsetX()
    {
        return ((((_value - Min) / (Max - Min)) * ContainerWidth) + (_thumbWidth / 2)).ToString(_nF);
    }

    #endregion

    #region Vertical

    private async Task OnContainerVerticalDownAsync(float pageY, bool isMouse)
    {
        if (_mouseDownOnCircle) return;

        _isMouse = isMouse;
        _showTooltip = true;
        _mouseDownOnCircle = true;
        
        if (isMouse)
        {
            await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("mouseup",  _containerId, OnVerticalUpAsync);
            await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("mousemove",  _containerId, OnVerticalMouseMoveAsync);
        }
        else
        {
            await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("touchend", _containerId, OnVerticalUpAsync);
            await JsUtilsService.AddDocumentListenerAsync<TouchEventArgs>("touchmove",  _containerId, OnVerticalTouchMoveAsync);
        }
        
        _thumb = await JsUtilsService.GetBoundingClientRectAsync(_circleId);

        _prevPageY = pageY;
        _offsetY = _container.Height - (pageY - _container.Top + _thumb.Height * 0.5f);
        _offsetY = Math.Clamp(_offsetY, 0, _container.Height - _thumb.Height);

        UpdateValue();
    }

    private async Task OnVerticalDownAsync(float pageY, bool isMouse)
    {
        _isMouse = isMouse;
        _showTooltip = true;
        _mouseDownOnCircle = true;
        _prevPageY = pageY;
        
        if (isMouse)
        {
            await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("mouseup",  _containerId, OnVerticalUpAsync);
            await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("mousemove",  _containerId, OnVerticalMouseMoveAsync);
        }
        else
        {
            await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("touchend", _containerId, OnVerticalUpAsync);
            await JsUtilsService.AddDocumentListenerAsync<TouchEventArgs>("touchmove",  _containerId, OnVerticalTouchMoveAsync);
        }
        
        _thumb = await JsUtilsService.GetBoundingClientRectAsync(_circleId);
    }

    [JSInvokable]
    public async Task OnVerticalUpAsync(MouseEventArgs _)
    {
        if (_isMouse)
        {
            await JsUtilsService.RemoveDocumentListenerAsync<MouseEventArgs>("mouseup", _containerId);
            await JsUtilsService.RemoveDocumentListenerAsync<MouseEventArgs>("mousemove", _containerId);
        }
        else
        {
            await JsUtilsService.RemoveDocumentListenerAsync<MouseEventArgs>("touchend", _containerId);
            await JsUtilsService.RemoveDocumentListenerAsync<TouchEventArgs>("touchmove", _containerId);
        }
        
        _showTooltip = false;
        _mouseDownOnCircle = false;

        StateHasChanged();
    }

    [JSInvokable]
    public Task OnVerticalMouseMoveAsync(MouseEventArgs mouseEvent)
    {
        OnVerticalMove((float) mouseEvent.PageY);

        return Task.CompletedTask;
    }

    [JSInvokable]
    public async Task OnVerticalTouchMoveAsync(TouchEventArgs touchEvent)
    {
        if (!_mouseDownOnCircle) return;

        if (touchEvent.Touches.Length == 1)
        {
            OnVerticalMove((float) touchEvent.Touches[0].PageY);
            return;
        }

        await OnVerticalUpAsync(new MouseEventArgs());
    }

    private void OnVerticalMove(float pageY)
    {
        if (!_mouseDownOnCircle) return;
        
        var dY = _prevPageY - pageY;

        _prevPageY = pageY;
        _offsetY += dY;
        _offsetY = Math.Clamp(_offsetY, 0, _container.Height - _thumb.Height);
        UpdateValue();
    }
    
    private string GetBottomOffset()
    {
        var bottomOffset = (ContainerHeight - (((_value - Min) / (Max - Min)) * ContainerHeight)).ToString(_nF);
        return bottomOffset;
    }

    #endregion

    private void UpdateValue()
    {
        if (Step > 0)
        {
            int stepIndex;
            
            if (Vertical) stepIndex = (int)((((_offsetY + _heightStep * 0.5) / ContainerHeight) * (Max - Min)) / Step);
            else stepIndex = (int)((((_offsetX + _widthStep * 0.5) / ContainerWidth) * (Max - Min)) / Step);

            _value = Step * stepIndex;
            ValueChanged.InvokeAsync(_value);
            StateHasChanged();
            return;
        }

        var points = Max - Min;

        _value = Vertical ? ((_offsetY / ContainerHeight) * points) + Min : ((_offsetX / ContainerWidth) * points) + Min;
        ValueChanged.InvokeAsync(_value);

        StateHasChanged();
    }

    private async Task OnUpdateAsync()
    {
        _container = await JsUtilsService.GetBoundingClientRectAsync(_containerId);
        _thumb = await JsUtilsService.GetBoundingClientRectAsync(_circleId);

        _container ??= new();
        _thumb ??= new();
        
        _thumbHeight = _thumb.Height;
        _thumbWidth = _thumb.Width;

        var points = Max - Min;

        if (Vertical) _offsetY = ((_value - Min) * ContainerHeight) / points;
        else _offsetX = ((_value - Min) * ContainerWidth) / points;
        
        if (Step > 0)
        {
            var stepCount = (Max - Min) / Step;
            _widthStep = ContainerWidth / stepCount;
            _heightStep = ContainerHeight / stepCount;
        }

        StateHasChanged();
    }

    private async Task OnResizeAsync()
    {
        await OnUpdateAsync();
    }

    public void Dispose()
    {
        IJSUtilsService.OnResize -= OnResizeAsync;
    }

    public async Task SetValueAsync(float value)
    {
        _value = value;
        await OnUpdateAsync();
    }
}
