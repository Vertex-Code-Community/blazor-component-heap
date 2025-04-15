using BlazorComponentHeap.Components.Cropper;
using BlazorComponentHeap.Components.Models.Cropper;
using BlazorComponentHeap.Components.Models.Zoom;
using BlazorComponentHeap.Components.Range;
using BlazorComponentHeap.Components.Zoom;
using BlazorComponentHeap.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.Components.ImageCropper;

public partial class BCHImageCropper : IAsyncDisposable
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private IJSUtilsService JsUtilsService { get; set; } = null!;

    [Parameter] public string BackgroundColor { get; set; } = "#ffffff";
    [Parameter] public string ResultFormat { get; set; } = "image/jpeg";
    [Parameter] public float CroppedWidth { get; set; } = 400;
    [Parameter] public string Base64Image { get; set; } = string.Empty;
    [Parameter] public float MinScale { get; set; } = 0.1f;
    [Parameter] public float MaxScale { get; set; } = 8.0f;
    [Parameter] public int MinRectangleWidth { get; set; } = 80;
    [Parameter] public int MinRectangleHeight { get; set; } = 80;
    [Parameter] public float ScaleFactor { get; set; } = 0.009f;
    [Parameter] public bool ScaleOnMouseWheel { get; set; } = false;
    [Parameter] public bool UseTouchRotation { get; set; } = false;
    [Parameter] public bool IsMobile { get; set; } = false;

    [Parameter] public EventCallback OnSaveClicked { get; set; }
    [Parameter] public EventCallback OnCancelClicked { get; set; }

    private readonly string _key = $"_id_{Guid.NewGuid()}";
    private BCHCropper _bchCropper = null!;
    private BCHRange _bchRange = null!;

    private bool _rotateCropper = false;
    private bool _isServerSide = false;

    protected override void OnInitialized()
    {
        _isServerSide = JsRuntime.GetType().Name.Contains("Remote");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsUtilsService.AddDocumentListenerAsync<object>("mouseup", _key, OnMouseUp);
            await JsUtilsService.AddDocumentListenerAsync<object>("touchend", _key, OnMouseUp);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await JsUtilsService.RemoveDocumentListenerAsync<object>("mouseup", _key);
        await JsUtilsService.RemoveDocumentListenerAsync<object>("touchend", _key);
    }
    
    private Task OnMouseUp(object _)
    {
        _rotateCropper = false;
        return Task.CompletedTask;
    }

    private async Task OnMouseDownAsync(float angleDelta)
    {
        _rotateCropper = true;
        await RotateAsync(angleDelta);
    }

    private async Task RotateAsync(float angleDelta)
    {
        if (!_rotateCropper) return;
        await Task.Delay(20);
        _bchCropper.Rotate(angleDelta);

        await RotateAsync(angleDelta);
    }

    private void OnRangeMove(float value)
    {
        _bchCropper.ScaleTo(value);
    }

    private async Task OnUpdateCropperAsync(float scale)
    {
        await _bchRange.SetValueAsync(scale);
    }

    private async Task OnSetRatioAsync(float ratio)
    {
        await _bchCropper.SetRectangleRatioAsync(ratio);
    }

    public Task<string> GetBase64ResultAsync()
    {
        return _bchCropper.GetBase64ResultAsync();
    }
}