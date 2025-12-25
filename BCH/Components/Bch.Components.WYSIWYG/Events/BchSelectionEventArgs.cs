using Bch.Modules.GlobalEvents.Models;

namespace Bch.Components.WYSIWYG.Events;

public class BchSelectionEventArgs : EventArgs
{
    public List<ElementParameters> PathCoordinates { get; set; } = new();
    public string? TextContent { get; set; }
}