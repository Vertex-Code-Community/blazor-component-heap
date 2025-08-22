using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Bch.Components.Cropper.Models;
using Bch.Components.Zoom;
using Bch.Modules.DomInterop.Services;
using Bch.Modules.GlobalEvents.Events;
using Bch.Modules.GlobalEvents.Services;
using Bch.Modules.Maths.Models;

namespace Bch.Components.Cropper;

public partial class BchCropper : IAsyncDisposable
{
    [Inject] public required IDomInteropService DomInteropService { get; set; }
    [Inject] public required IGlobalEventsService GlobalEventsService { get; set; }
    [Inject] public required IJSRuntime JsRuntime { get; set; }

    [Parameter, EditorRequired] public required string ImageUrl { get; set; }
    [Parameter] public CropperType CropperType { get; set; } = CropperType.MovableRectangle;
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

    // private readonly ZoomContext _zoomContext = new();
    private BchZoom? _bchZoom;

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
    
    protected override Task OnInitializedAsync()
    {
        // IJSUtilsService.OnResize += OnResizeAsync;
        // _zoomContext.OnUpdate += OnUpdateZoom;
        
        return GlobalEventsService.AddDocumentListenerAsync<BchWheelEventArgs>("mousewheel", _key, OnMouseWheel, 
            false, false, false);
    }
    
    public async ValueTask DisposeAsync()
    {
        // IJSUtilsService.OnResize -= OnResizeAsync;
        // _zoomContext.OnUpdate -= OnUpdateZoom;
        await GlobalEventsService.RemoveDocumentListenerAsync<BchWheelEventArgs>("mousewheel", _key);
    }

    private async Task OnUpdateZoom()
    {
        // if (_scaleLinear != _zoomContext.ScaleLinear)
        // {
        //     _scaleLinear = _zoomContext.ScaleLinear;
        //     if (OnUpdateScale is not null)
        //     {
        //         await OnUpdateScale.Invoke(_zoomContext.ScaleLinear);
        //     }
        // }
        
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

    public void Rotate(float angleDelta)
    {
        //_bchZoom?.Transform(_rectPos.X + _rectSize.X * 0.5f, _rectPos.Y + _rectSize.Y * 0.5f, 0, angleDelta);
    }

    public void ScaleTo(float scale)
    {
        // var scaleDelta = (scale - _zoomContext.ScaleLinear) / ScaleFactor;
        // _bchZoom?.Transform(_rectPos.X + _rectSize.X * 0.5f, _rectPos.Y + _rectSize.Y * 0.5f, scaleDelta, 0);
    }

    public async Task SetRectangleRatioAsync(float ratio)
    {
        if (CropperType is CropperType.MovableRectangle or CropperType.FixedRectangle)
        {
            var rect = await DomInteropService.GetBoundingClientRectAsync(_cropperId);
            if (rect is null) return;
            
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