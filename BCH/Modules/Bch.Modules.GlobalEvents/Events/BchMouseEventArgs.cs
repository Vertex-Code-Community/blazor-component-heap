using Microsoft.AspNetCore.Components.Web;
using Bch.Modules.GlobalEvents.Models;

namespace Bch.Modules.GlobalEvents.Events;

public class BchMouseEventArgs : MouseEventArgs
{
    public List<ElementParameters> PathCoordinates { get; set; } = new();

    public float X { get; set; }
    public float Y { get; set; }
    public double ClientWidth { get; set; }
    public double ClientHeight { get; set; }
    public string TargetClassList { get; set; } = string.Empty;
    public string RelatedTargetClassList { get; set; } = string.Empty;
    public bool RelatedTargetIsChildOfTarget { get; set; }
}