using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Bch.Modules.Maths.Models;
using Microsoft.AspNetCore.Components.Forms;
using Bch.Modules.Files;
using Bch.Modules.Files.Models;

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
    
    public async Task<BchFilesContext> GetFileResultAsync(string fileName = "cropped-image", float quality = 1.0f)
    {
        _processing = true;
        await ProcessingChanged.InvokeAsync(_processing);
        StateHasChanged();
        
        var imageRect = await DomInteropService.GetBoundingClientRectAsync(_imageId);
        if (imageRect is null) return new BchFilesContext();

        var imgPixelRatio = imageRect.OffsetWidth / imageRect.Width;
        
        var imgBounds = new Vec2 { X = imageRect.OffsetWidth, Y = imageRect.OffsetHeight };
        var fileInfo = await JsRuntime.InvokeAsync<BchFileInfoModel?>("bchOnCropImage", _canvasId,
            _canvasHolderId, _imageId, new { x = _zoomTopLeftPosX, y = _zoomTopLeftPosY }, imgBounds, _zoomAngleInRadians,
            _zoomScale, ResultFormat, quality, BackgroundColor, imgPixelRatio, _rectPos, _rectSize, fileName);
        
        var context = new BchFilesContext();
        if (fileInfo is not null)
        {
            context.Files.Add(new BchBrowserFile
            {
                JsRuntime = JsRuntime,
                Id = fileInfo.Id,
                Name = fileInfo.Name,
                ContentType = fileInfo.ContentType,
                Size = fileInfo.Size,
                LastModified = fileInfo.LastModified,
                ImagePreviewUrl = fileInfo.ImagePreviewUrl
            });
        }
        
        _processing = false;
        await ProcessingChanged.InvokeAsync(_processing);
        StateHasChanged();
        
        return context;
    }
}