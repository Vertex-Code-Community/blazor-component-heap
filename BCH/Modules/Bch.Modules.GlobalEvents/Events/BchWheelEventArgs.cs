using Microsoft.AspNetCore.Components.Web;
using Bch.Modules.GlobalEvents.Models;

namespace Bch.Modules.GlobalEvents.Events;

public class BchWheelEventArgs : WheelEventArgs
{
    public float X { get; set; }
    public float Y { get; set; }

    public List<ElementParameters> PathCoordinates { get; set; } = new();
}