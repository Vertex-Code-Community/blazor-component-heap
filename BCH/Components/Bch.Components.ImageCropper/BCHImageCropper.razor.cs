using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Bch.Components.Cropper;
using Bch.Components.Range;
using Bch.Modules.GlobalEvents.Services;

namespace Bch.Components.ImageCropper;

public partial class BchImageCropper : IAsyncDisposable
{
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    [Inject] public required IGlobalEventsService GlobalEventsService { get; set; }

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
    private BchCropper _bchCropper = null!;
    private BchRange _bchRange = null!;

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
            await GlobalEventsService.AddDocumentListenerAsync<object>("mouseup", _key, OnMouseUp);
            await GlobalEventsService.AddDocumentListenerAsync<object>("touchend", _key, OnMouseUp);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await GlobalEventsService.RemoveDocumentListenerAsync<object>("mouseup", _key);
        await GlobalEventsService.RemoveDocumentListenerAsync<object>("touchend", _key);
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

    // TODO: make as file stream
    public Task<string> GetBase64ResultAsync()
    {
        return _bchCropper.GetBase64ResultAsync();
    }
}