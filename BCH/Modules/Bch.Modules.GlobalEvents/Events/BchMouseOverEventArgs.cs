using Microsoft.AspNetCore.Components.Web;
using Bch.Modules.GlobalEvents.Models;

namespace Bch.Modules.GlobalEvents.Events;

public class BchMouseOverEventArgs : MouseEventArgs
{
    public List<ElementParameters> PathCoordinates { get; set; } = new();
}