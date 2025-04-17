using BlazorComponentHeap.Core.Models.Markup;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorComponentHeap.Core.Models.Events;

public class MouseOverEventArgs : MouseEventArgs
{
    public List<CoordsHolder> PathCoordinates { get; set; } = new();
}
