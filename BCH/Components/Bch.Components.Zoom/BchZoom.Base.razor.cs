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
    [Parameter] public float ScaleFactor { get; set; } = 0.009f;
    [Parameter] public float MinScale { get; set; } = 2.0f;
    [Parameter] public float MaxScale { get; set; } = 6.0f;
    [Parameter] public bool UseTouchRotation { get; set; } = false;
    [Parameter] public bool ZoomOnMouseWheel { get; set; } = false;
    
    [Parameter] public float? InitialScale { get; set; }

    private readonly string _wrapperId = $"_id_{Guid.NewGuid()}";
    private readonly string _navigationId = $"_id_{Guid.NewGuid()}";
    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };

    private readonly Vec2 _viewPortSize = new();
    private readonly Vec2 _navigationSize = new();
    private readonly Vec2 _navigationOffsetSize = new();
    private float _scale = 4;

    private bool _changePerformed = false;

    private float _dppx = 1.0f;
    private readonly string _key = $"_id_{Guid.NewGuid()}";
    
    protected override Task OnInitializedAsync()
    {
        if (InitialScale is not null)
        {
            var scale = InitialScale.Value;
            _scale = (float) Math.Log(scale) + 4;
            // TODO: change it
            MinScale = _scale;
        }
        
        return GlobalEventsService.AddDocumentListenerAsync<BchWheelEventArgs>("mousewheel", _key, OnMouseWheelAsync, 
            false, false, false);
    }

    public async ValueTask DisposeAsync()
    {
        await GlobalEventsService.RemoveDocumentListenerAsync<BchWheelEventArgs>("mousewheel", _key);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dppx = await JsRuntime.InvokeAsync<float>("bchGetPixelRatio");
            
            var wrapperRect = await DomInteropService.GetBoundingClientRectAsync(_wrapperId);
            if (wrapperRect is null) return;
        
            var navigationRect = await DomInteropService.GetBoundingClientRectAsync(_navigationId);
            if (navigationRect is null) return;

            _viewPortSize.Set(wrapperRect.Width, wrapperRect.Height);
            _navigationSize.Set(navigationRect.Width, navigationRect.Height);
            _navigationOffsetSize.Set(navigationRect.OffsetWidth, navigationRect.OffsetHeight);

            Update();
        }

        if (_changePerformed)
        {
            _changePerformed = false;

            var navigationRect = await DomInteropService.GetBoundingClientRectAsync(_navigationId);
            if (navigationRect is null) return;

            _navigationSize.Set(navigationRect.Width, navigationRect.Height);
            _navigationOffsetSize.Set(navigationRect.OffsetWidth, navigationRect.OffsetHeight);
        }
    }
    
    protected override void OnParametersSet()
    {
        MaxScale = Math.Clamp(MaxScale, 0.01f, 50f);
        MinScale = Math.Clamp(MinScale, 0.01f, 50f);

        if (MinScale >= MaxScale)
        {
            MinScale = 1.0f;
            MaxScale = 4.0f;
        }

        ScaleFactor = Math.Clamp(ScaleFactor, 0.001f, 1.0f);

        Update();
    }

    public async Task CenterContentAsync()
    {
        // if (ViewMode == ViewMode.Default) return;
        
        var wrapperRect = await DomInteropService.GetBoundingClientRectAsync(_wrapperId);
        var navigationRect = await DomInteropService.GetBoundingClientRectAsync(_navigationId);
        if (wrapperRect is null || navigationRect is null) return;
        
        _viewPortSize.Set(wrapperRect.Width, wrapperRect.Height);
        _navigationSize.Set(navigationRect.Width, navigationRect.Height);
        _navigationOffsetSize.Set(navigationRect.OffsetWidth, navigationRect.OffsetHeight);

        if (_navigationSize.X == 0 || _navigationSize.Y == 0) return;
        
        var scale = _viewPortSize.X / _navigationSize.X;
        var newHeight = _navigationSize.Y * scale;
        
        var x = 0.0f;
        var y = 0.0f;
        // var y = _viewPortSize.Y * 0.5f - newHeight * 0.5f;
        //
        // var heightCondition = ViewMode == ViewMode.StretchToBorders
        //     ? newHeight > _viewPortSize.Y
        //     : newHeight < _viewPortSize.Y;
        //
        // if (heightCondition)
        // {
        //     scale = _viewPortSize.Y / _navigationSize.Y;
        //     var newWidth = _navigationSize.X * scale;
        //     
        //     x = _viewPortSize.X * 0.5f - newWidth * 0.5f;
        //     y = 0;
        // }
        //
        // if (ViewMode == ViewMode.StretchByWidth)
            // y = 0;

        _pos.Set(x, y);
        _scale = (float) Math.Log(scale) + 4;
        
        // TODO: change it
        MinScale = _scale;
        
        // Console.WriteLine($"wrapperWidth = {_viewPortSize.X}, navigationWidth = {_navigationSize.X}");
        // Console.WriteLine($"newHeight = {newHeight}, _viewPortSize.Y = {_viewPortSize.Y}");
        // Console.WriteLine($"_scale = {_scale}, scale = {scale}");

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
            UserInteracttion = _userInteraction
        });
    }

    private float Scale => (float)Math.Exp(_scale - 4);

    private string GetNavigationStyle()
    {
        return ($"transform:" +
                $"translate({_pos.X.ToString(_nF)}px, {_pos.Y.ToString(_nF)}px) " +
                $"scale({Scale.ToString(_nF)}) " +
                $"rotate({(_rotationAngle * (180 / Math.PI)).ToString(_nF)}deg); " +
                $"{(true ? "transition: transform 0s; cursor: move;" : "")}");
    }
}