using Bch.Components.Zoom;
using Bch.Modules.Maths.Models;

namespace Bch.Integration.Pages.Pages.Zoom;

public partial class ZoomPage
{
    private BchZoom? _bchZoom2;
    
    private readonly Vec2 _zoomContainerSize1 = new(308, 206);
    private readonly Vec2 _zoomContainerSize2 = new(308 * 1.5f, 206 * 1.5f);
    private readonly Vec2 _zoomContainerSize3 = new(300, 100);
    private readonly Vec2 _zoomContainerSize4 = new(100, 400);
    private readonly Vec2 _imageSize1 = new(413, 276);
}