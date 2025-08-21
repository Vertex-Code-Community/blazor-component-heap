using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Bch.Components.Zoom.Models;
using Bch.Modules.DomInterop.Services;
using Bch.Modules.GlobalEvents.Events;
using Bch.Modules.GlobalEvents.Services;
using Bch.Modules.Maths.Models;

namespace Bch.Components.Zoom;

public partial class BchZoom : IAsyncDisposable
{
    [Inject] public required IDomInteropService DomInteropService { get; set; }
    [Inject] public required IGlobalEventsService GlobalEventsService { get; set; }
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    
    [Parameter] public required RenderFragment<ZoomUpdateSnapshot> ChildContent { get; set; }
    [Parameter] public EventCallback<ZoomUpdateSnapshot> OnUpdate { get; set; }
    [Parameter] public ConstraintType Constraint { get; set; } = ConstraintType.None;
    [Parameter, EditorRequired] public required Vec2 ContainerSize { get; set; }
    [Parameter, EditorRequired] public required Vec2 ContentSize { get; set; }
    [Parameter] public float ScaleFactor { get; set; } = 0.009f;
    [Parameter] public float MinScale { get; set; } = 0.5f;
    [Parameter] public float MaxScale { get; set; } = 10.0f;
    [Parameter] public bool UseTouchRotation { get; set; } = false;
    [Parameter] public bool ZoomOnMouseWheel { get; set; } = false;
    
    private float Scale => (float)Math.Exp(_scale - 4);

    private readonly string _wrapperId = $"_id_{Guid.NewGuid()}";
    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };

    private readonly Vec2 _viewPortSize = new();
    private readonly Vec2 _navigationOffsetSize = new();
    private float _scale = 4;

    private float _dppx = 1.0f;
    private readonly string _key = $"_id_{Guid.NewGuid()}";

    private float _minScale;
    private float _maxScale;
    private float _scaleFactor;
    private ConstraintType _constraint;
    
    protected override Task OnInitializedAsync()
    {
        _minScale = Math.Clamp(MinScale, 0.01f, 50f);
        _maxScale = Math.Clamp(MaxScale, 0.01f, 50f);
        _scaleFactor = Math.Clamp(ScaleFactor, 0.001f, 1.0f);
        _constraint = Constraint;

        _viewPortSize.Set(ContainerSize);
        _navigationOffsetSize.Set(ContentSize);
        
        // calculate min/max limits in case of using constraints
        // var xLimitScale = ContainerSize.X / ContentSize.X;
        // var yLimitScale = ContainerSize.Y / ContentSize.Y;
        
        // if (_constraint == ConstraintType.Outside)
        //     _minScale = (float) Math.Log(Math.Max(xLimitScale, yLimitScale)) + 4;
        //
        // if (_constraint == ConstraintType.Inside)
        //     _maxScale = (float) Math.Log(Math.Min(xLimitScale, yLimitScale)) + 4;
        
        ApplyInitialPositioning();
        
        Update();

        return GlobalEventsService.AddDocumentListenerAsync<BchWheelEventArgs>("mousewheel", _key, OnMouseWheelAsync, 
            false, false, false);
    }

    public async ValueTask DisposeAsync()
    {
        await GlobalEventsService.RemoveDocumentListenerAsync<BchWheelEventArgs>("mousewheel", _key);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        _dppx = await JsRuntime.InvokeAsync<float>("bchGetPixelRatio");
    }
    
    protected override void OnParametersSet()
    {
        Update();
    }

    private void Update()
    {
        ApplyConstraints();
        InvokeUpdateCallback();
        
        StateHasChanged();
    }

    private void InvokeUpdateCallback()
    {
        OnUpdate.InvokeAsync(new ZoomUpdateSnapshot
        {
            PosX = _pos.X,
            PosY = _pos.Y,
            Scale = Scale,
            ScaleLinear = _scale,
            AngleInRadians = _rotationAngle,
            UserInteraction = _userInteraction
        });
    }

    private string GetNavigationStyle()
    {
        return ($"transform:" +
                $"translate({_pos.X.ToString(_nF)}px, {_pos.Y.ToString(_nF)}px) " +
                $"scale({Scale.ToString(_nF)}) " +
                $"rotate({(_rotationAngle * (180 / Math.PI)).ToString(_nF)}deg); " +
                $"{(true ? "transition: transform 0s; cursor: move;" : "")}");
    }
}