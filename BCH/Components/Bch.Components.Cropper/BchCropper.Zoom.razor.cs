using Bch.Components.Cropper.Events;
using Bch.Components.Zoom.Models;
using Bch.Modules.Maths.Models;

namespace Bch.Components.Cropper;

public partial class BchCropper
{
    private readonly Vec2 _containerSize = new(0, 0);
    private readonly Vec2 _contentSize = new(0, 0);
    private bool _userInteraction;
    
    private async Task OnImageLoadedAsync(ImageLoadedEventArgs e)
    {
        var containerRect = await DomInteropService.GetBoundingClientRectAsync(_cropperId);
        if (containerRect is null) return;
        
        Console.WriteLine($"w = {containerRect.Width}, h = {containerRect.Height}");
        Console.WriteLine($"im-w = {e.ImageWidth}, im-h = {e.ImageHeight}");

        _containerSize.Set(containerRect.Width, containerRect.Height);
        _contentSize.Set(e.ImageWidth, e.ImageHeight);
        
        _loaded = true;
        StateHasChanged();
    }

    private void OnZoomUpdate(ZoomUpdateSnapshot e)
    {
        // var prevUserInteraction = _userInteraction;
        // _userInteraction = e.UserInteraction;
        //
        // if (prevUserInteraction != _userInteraction)
        //     StateHasChanged();
    }
}