using Bch.Modules.GlobalEvents.Models;

namespace Bch.Modules.GlobalEvents.Events;

public class BchTouchEventArgs : EventArgs
{
    public List<BchTouchPoint> Touches { get; set; } = new();
    public List<ElementParameters> PathCoordinates { get; set; } = new();
}