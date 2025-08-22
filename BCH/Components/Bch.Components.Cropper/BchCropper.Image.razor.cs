using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Bch.Modules.Maths.Models;

namespace Bch.Components.Cropper;

public partial class BchCropper
{
    [Parameter] public EventCallback<bool> ProcessingChanged { get; set; }
    [Parameter] public bool Processing { get => _processing; set { } }
    
    private bool _processing = false;

    private float _zoomTopLeftPosX;
    private float _zoomTopLeftPosY;
    private float _zoomScale;
    private float _zoomAngleInRadians;
    
    public async Task<string> GetBase64ResultAsync()
    {
        _processing = true;
        await ProcessingChanged.InvokeAsync(_processing);
        StateHasChanged();
        
        var imageRect = await DomInteropService.GetBoundingClientRectAsync(_imageId);
        if (imageRect is null) return string.Empty;
        
        var imgBounds = new Vec2 { X = imageRect.OffsetWidth,Y = imageRect.OffsetHeight };
        // TODO: make as file stream
        var base64Result = await JsRuntime.InvokeAsync<string>("bchOnCropImage", _canvasId, 
            _canvasHolderId, _imageId, new { x = _zoomTopLeftPosX, y = _zoomTopLeftPosY }, imgBounds, _zoomAngleInRadians, 
            _zoomScale, ResultFormat, 1.0f, BackgroundColor, CroppedWidth, _rectPos, _rectSize);
        
        _processing = true;
        await ProcessingChanged.InvokeAsync(_processing);
        StateHasChanged();
        
        //return base64Result;

        return string.Empty;
    }
}