using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Bch.Components.Zoom;
using Bch.Components.Zoom.Models;
using Bch.Modules.DomInterop.Services;
using Bch.Modules.GlobalEvents.Services;
using Bch.Modules.GlobalEvents.Events;
using Bch.Modules.Maths.Models;

namespace Bch.Components.Panzoom;

public partial class BchPanzoom : ComponentBase, IAsyncDisposable
{
    [Inject] public required IDomInteropService DomInteropService { get; set; }
    [Inject] public required IGlobalEventsService GlobalEventsService { get; set; }

    [Parameter] public string BackgroundColor { get; set; } = "#ffffff";
    [Parameter] public required string ImageUrl { get; set; }
    [Parameter] public float MinScale { get; set; } = 2.0f;
    [Parameter] public float MaxScale { get; set; } = 6.0f;
    [Parameter] public int MinRectangleWidth { get; set; } = 80;
    [Parameter] public int MinRectangleHeight { get; set; } = 80;
    [Parameter] public float ScaleFactor { get; set; } = 0.009f;
    [Parameter] public bool ScaleOnMouseWheel { get; set; } = false;
    [Parameter] public bool UseTouchRotation { get; set; } = false;
    [Parameter] public Func<float, Task>? OnUpdateScale { get; set; }

    private BchZoom? _bchZoom;
    private bool _processingData = false;

    private readonly string _cropperId = $"_id_{Guid.NewGuid()}";
    private readonly string _imageId = $"_id_{Guid.NewGuid()}";
    private readonly string _key = $"_id_{Guid.NewGuid()}";

    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };
    private readonly Vec2 _lastMousePosition = new();
    private readonly Vec2 _change = new();
    private readonly MouseEventArgs _eventObj = new();
    
    private bool _isZoomConstrained = false;
    private bool _leftLock = false;
    private bool _rightLock = false;
    private bool _topLock = false;
    private bool _bottomLock = false;

    private bool _loaded = false;
    private float _scaleLinear = -1;
    
    protected override Task OnInitializedAsync()
    {
        return GlobalEventsService.AddDocumentListenerAsync<BchWheelEventArgs>("mousewheel", _key, OnMouseWheel, 
            false, false, false);
    }
    
    public async ValueTask DisposeAsync()
    {
        // IJSUtilsService.OnResize -= OnResizeAsync;
        await GlobalEventsService.RemoveDocumentListenerAsync<BchWheelEventArgs>("mousewheel", _key);
    }
    
    private Task OnMouseWheel(BchWheelEventArgs e)
    {
        if (!ScaleOnMouseWheel || _bchZoom is null) return Task.CompletedTask;

        var wrapper = e.PathCoordinates.FirstOrDefault(x => x.Id == _cropperId);
        if (wrapper is null) return Task.CompletedTask;

        _bchZoom.Transform(wrapper.X, wrapper.Y, -(float)e.DeltaY, 0);
        return Task.CompletedTask;
    }

    
    protected override void OnParametersSet()
    {
        if (MinRectangleWidth < 50) MinRectangleWidth = 50;
        if (MinRectangleHeight < 50) MinRectangleHeight = 50;
    }
}


