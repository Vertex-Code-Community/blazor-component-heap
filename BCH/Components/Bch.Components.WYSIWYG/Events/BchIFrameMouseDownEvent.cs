using Microsoft.AspNetCore.Components.Web;
using Bch.Modules.DomInterop.Models;
using Bch.Modules.GlobalEvents.Models;

namespace Bch.Components.WYSIWYG.Events;

public class BchIFrameMouseDownEvent : MouseEventArgs
{
    public ElementParameters? CoordsHolder { get; set; }
    public BoundingClientRect? Rect { get; set; }
    public bool HasTableTag { get; set; }
}