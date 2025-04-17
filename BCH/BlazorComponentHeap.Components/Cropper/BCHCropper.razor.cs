using System.Globalization;
using BlazorComponentHeap.Components.Models.Cropper;
using BlazorComponentHeap.Components.Models.Zoom;
using BlazorComponentHeap.Components.Zoom;
using BlazorComponentHeap.Core.Models.Events;
using BlazorComponentHeap.Core.Models.Math;
using BlazorComponentHeap.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.Components.Cropper;

public partial class BCHCropper : IAsyncDisposable
{
    [Inject] private IJSUtilsService JsUtilsService { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter] public string Base64Image { get; set; } = string.Empty;
    [Parameter] public CropperType CropperType { get; set; } = CropperType.MovableRectangle;
    [Parameter] public ViewMode ViewMode { get; set; } = ViewMode.StretchToBorders;
    [Parameter] public string BackgroundColor { get; set; } = "#ffffff";
    [Parameter] public string ResultFormat { get; set; } = "image/jpeg";
    [Parameter] public float CroppedWidth { get; set; } = 400;
    [Parameter] public float Ratio { get; set; } = 1;
    [Parameter] public float MinScale { get; set; } = 2.0f;
    [Parameter] public float MaxScale { get; set; } = 6.0f;
    [Parameter] public int MinRectangleWidth { get; set; } = 80;
    [Parameter] public int MinRectangleHeight { get; set; } = 80;
    [Parameter] public float ScaleFactor { get; set; } = 0.009f;
    [Parameter] public bool StretchCropArea { get; set; } = false;
    [Parameter] public bool ScaleOnMouseWheel { get; set; } = false;
    [Parameter] public bool UseTouchRotation { get; set; } = false;
    [Parameter] public Func<float, Task>? OnUpdateScale { get; set; }

    private readonly ZoomContext _zoomContext = new();
    private BCHZoom? _bchZoom;
    private bool _processingData = false;

    private readonly string _cropperId = $"_id_{Guid.NewGuid()}";
    private readonly string _imageId = $"_id_{Guid.NewGuid()}";
    private readonly string _canvasId = $"_id_{Guid.NewGuid()}";
    private readonly string _canvasHolderId = $"_id_{Guid.NewGuid()}";
    private readonly string _rectId = $"_id_{Guid.NewGuid()}";
    private readonly string _circleId = $"_id_{Guid.NewGuid()}";
    private readonly string _key = $"_id_{Guid.NewGuid()}";
    private int _circleSize = 0;
    private int _cropperWidth = 0;
    private int _cropperHeight = 0;

    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };
    private bool _rectDragged = false;
    private bool _rectHandleDragged = false;
    private readonly Vec2 _rectPos = new();
    private readonly Vec2 _lastMousePosition = new();
    private readonly Vec2 _change = new();
    private readonly Vec2 _rectSize = new() { X = 100, Y = 100 };
    private readonly MouseEventArgs _eventObj = new();

    private bool _loaded = false;
    private float _scaleLinear = -1;
    private bool _isServerSide = false;
    
    protected override void OnInitialized()
    {
        _isServerSide = JsRuntime.GetType().Name.Contains("Remote"); // JsRuntime is IJSInProcessRuntime - webassembly, WebViewJsRuntime - MAUI
        
        IJSUtilsService.OnResize += OnResizeAsync;
        _zoomContext.OnUpdate += OnUpdateZoom;
    }
    
    public async ValueTask DisposeAsync()
    {
        IJSUtilsService.OnResize -= OnResizeAsync;
        _zoomContext.OnUpdate -= OnUpdateZoom;
        
        await JsUtilsService.RemoveDocumentListenerAsync<ExtWheelEventArgs>("mousewheel", _key);
    }
    
    [JSInvokable]
    public Task OnMouseWheel(ExtWheelEventArgs e)
    {
        if (!ScaleOnMouseWheel || _bchZoom is null) return Task.CompletedTask;

        var movableId = CropperType == CropperType.Circle ? _circleId : _rectId;
        
        var rectWrapper = e.PathCoordinates.FirstOrDefault(x => x.Id == movableId);
        if (rectWrapper is null) return Task.CompletedTask;
        
        var wrapper = e.PathCoordinates.FirstOrDefault(x => x.Id == _cropperId);
        if (wrapper is null) return Task.CompletedTask;

        _bchZoom.Transform(wrapper.X, wrapper.Y, -(float)e.DeltaY, 0);
        return Task.CompletedTask;
    }

    private async Task OnUpdateZoom()
    {
        if (_scaleLinear != _zoomContext.ScaleLinear)
        {
            _scaleLinear = _zoomContext.ScaleLinear;
            if (OnUpdateScale is not null)
            {
                await OnUpdateScale.Invoke(_zoomContext.ScaleLinear);
            }
        }
        
        StateHasChanged();
    }
    
    protected override void OnParametersSet()
    {
        if (MinRectangleWidth < 50) MinRectangleWidth = 50;
        if (MinRectangleHeight < 50) MinRectangleHeight = 50;
        
        if (CropperType == CropperType.Circle)
        {
            _rectSize.X = _circleSize;
            _rectSize.Y = _circleSize;

            _rectPos.X = (_cropperWidth - _circleSize) * 0.5f;
            _rectPos.Y = (_cropperHeight - _circleSize) * 0.5f;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsUtilsService.AddDocumentListenerAsync<ExtWheelEventArgs>("mousewheel", _key, OnMouseWheel, 
                false, false, false);
            
            await OnResizeAsync();

            if (StretchCropArea)
            {
                _rectSize.X = _circleSize;
                _rectSize.Y = _circleSize;

                _rectPos.X = (_cropperWidth - _circleSize) * 0.5f;
                _rectPos.Y = (_cropperHeight - _circleSize) * 0.5f;
                
                StateHasChanged();
            }
        }
    }

    private async Task OnResizeAsync()
    {
        var rect = await JsUtilsService.GetBoundingClientRectAsync(_cropperId);

        if (rect is not null)
        {
            _cropperWidth = (int) rect.Width;
            _cropperHeight = (int) rect.Height;
            _circleSize = Math.Min(_cropperWidth, _cropperHeight);
        }

        if (CropperType == CropperType.Circle)
        {
            _rectSize.X = _circleSize;
            _rectSize.Y = _circleSize;

            _rectPos.X = (_cropperWidth - _circleSize) * 0.5f;
            _rectPos.Y = (_cropperHeight - _circleSize) * 0.5f;
        }
        
        if (CropperType == CropperType.FixedRectangle)
        {
            var ratio = Ratio > 0 ? Ratio : 1.0f;

            var h2 = 1.0f;
            var w2 = ratio;

            var vertScale = _cropperHeight / h2;
            var horScale = _cropperWidth / w2;
            
            var scale = Math.Min(vertScale, horScale);

            var nH2 = scale * h2;
            var nW2 = scale * w2;
            
            _rectSize.X = nW2;
            _rectSize.Y = nH2;

            _rectPos.X = (_cropperWidth - nW2) * 0.5f;
            _rectPos.Y = (_cropperHeight - nH2) * 0.5f;
        }

        StateHasChanged();
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
        await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("mouseup", _key, OnMouseLeaveUpAsync);
        await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("touchend", _key, OnMouseLeaveUpAsync);
        await JsUtilsService.AddDocumentListenerAsync<MouseEventArgs>("mousemove", _key, OnMouseMoveAsync);
        await JsUtilsService.AddDocumentListenerAsync<TouchEventArgs>("touchmove", _key, OnTouchMoveAsync);
        
        _lastMousePosition.Set(e.PageX, e.PageY);
        _rectDragged = rectDragged;
        _rectHandleDragged = !rectDragged;
        
        StateHasChanged();
    }
    
    private async Task OnMouseLeaveUpAsync(MouseEventArgs _)
    {
        await JsUtilsService.RemoveDocumentListenerAsync<MouseEventArgs>("mouseup", _key);
        await JsUtilsService.RemoveDocumentListenerAsync<MouseEventArgs>("touchend", _key);
        await JsUtilsService.RemoveDocumentListenerAsync<MouseEventArgs>("mousemove", _key);
        await JsUtilsService.RemoveDocumentListenerAsync<TouchEventArgs>("touchmove", _key);
        
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

    public async Task<string> GetBase64ResultAsync()
    {
        if (_isServerSide) return string.Empty;
        
        _processingData = true;
        StateHasChanged();
        
        var imageRect = await JsUtilsService.GetBoundingClientRectAsync(_imageId);
        var imgBounds = new Vec2 { X = (float)imageRect.OffsetWidth,Y = (float)imageRect.OffsetHeight };
            
        var base64Result = await JsRuntime.InvokeAsync<string>("bchOnCropImage", _canvasId, 
            _canvasHolderId, _imageId, _zoomContext.TopLeftPos, imgBounds, _zoomContext.AngleInRadians, 
            _zoomContext.Scale, ResultFormat, 1.0f, BackgroundColor, CroppedWidth, _rectPos, _rectSize);

        _processingData = false;
        StateHasChanged();
        
        return base64Result;
    }

    public void Rotate(float angleDelta)
    {
        _bchZoom?.Transform(_rectPos.X + _rectSize.X * 0.5f, _rectPos.Y + _rectSize.Y * 0.5f, 0, angleDelta);
    }

    public void ScaleTo(float scale)
    {
        var scaleDelta = (scale - _zoomContext.ScaleLinear) / ScaleFactor;
        _bchZoom?.Transform(_rectPos.X + _rectSize.X * 0.5f, _rectPos.Y + _rectSize.Y * 0.5f, scaleDelta, 0);
    }

    public async Task SetRectangleRatioAsync(float ratio)
    {
        if (_isServerSide) return;
        
        if (CropperType is CropperType.MovableRectangle or CropperType.FixedRectangle)
        {
            var rect = await JsUtilsService.GetBoundingClientRectAsync(_cropperId);
            _cropperWidth = (int) rect.Width;
            _cropperHeight = (int) rect.Height;
            _circleSize = Math.Min(_cropperWidth, _cropperHeight);
            
            ratio = ratio > 0 ? ratio : 1.0f;

            var h2 = 1.0f;
            var w2 = ratio;

            var prevRectSizeX = _rectSize.X;
            var prevRectSizeY = _rectSize.Y;
            
            var horScale = (_cropperWidth - 10) / w2;
            var vertScale = (_cropperHeight - 10) / h2;
            
            var scale = Math.Min(vertScale, horScale);

            var nW2 = scale * w2;
            var nH2 = scale * h2;
            
            _rectSize.X = nW2;
            _rectSize.Y = nH2;
            
            _rectPos.X += (prevRectSizeX - _rectSize.X) * 0.5f;
            _rectPos.Y += (prevRectSizeY - _rectSize.Y) * 0.5f;
            
            StateHasChanged();
        }
    }
}