namespace BlazorComponentHeap.Core.Models.Markup;

public class CoordsHolder
{
    public string Id { get; set; } = string.Empty;
    public string ClassList { get; set; } = string.Empty;
    public float X { get; set; }
    public float Y { get; set; }
    public float ClientWidth { get; set; }
    public float ClientHeight { get; set; }
    public float ScrollTop { get; set; }
}
