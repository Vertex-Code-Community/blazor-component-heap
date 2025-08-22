using Bch.Components.Cropper.Events;
using Bch.Components.Cropper.Models;
using Bch.Components.Zoom.Models;
using Bch.Modules.Maths.Models;

namespace Bch.Components.Cropper;

public partial class BchCropper
{
    private readonly Vec2 _containerSize = new(0, 0);
    private readonly Vec2 _contentSize = new(0, 0);
    private bool _userInteraction;
    
    private async Task OnPreviewImageLoadedAsync(ImageLoadedEventArgs e)
    {
        var containerRect = await DomInteropService.GetBoundingClientRectAsync(_cropperId);
        if (containerRect is null) return;
        
        _cropperWidth = (int) containerRect.Width;
        _cropperHeight = (int) containerRect.Height;
        _circleSize = Math.Min(_cropperWidth, _cropperHeight);
        
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
            
            _rectSize.X = nW2 - 4;
            _rectSize.Y = nH2 - 4;

            _rectPos.X = (_cropperWidth - nW2) * 0.5f;
            _rectPos.Y = (_cropperHeight - nH2) * 0.5f;
        }

        _containerSize.Set(containerRect.Width, containerRect.Height);
        _contentSize.Set(e.ImageWidth, e.ImageHeight);
        
        _loaded = true;
        StateHasChanged();
    }

    private void OnZoomUpdate(ZoomUpdateSnapshot e)
    {
        var prevUserInteraction = _userInteraction;
        _userInteraction = e.UserInteraction;

        _zoomTopLeftPosX = e.PosX;
        _zoomTopLeftPosY = e.PosY;
        _zoomAngleInRadians = e.AngleInRadians;
        _zoomScale = e.Scale;
        
        if (prevUserInteraction != _userInteraction)
            StateHasChanged();
    }
}