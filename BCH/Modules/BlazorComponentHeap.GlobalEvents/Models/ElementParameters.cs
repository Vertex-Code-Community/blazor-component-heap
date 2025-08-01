namespace BlazorComponentHeap.GlobalEvents.Models;

public class ElementParameters
{
    public string Id { get; set; } = string.Empty;
    public string ClassList { get; set; } = string.Empty;
    public float X { get; set; }
    public float Y { get; set; }
    public float ClientWidth { get; set; }
    public float ClientHeight { get; set; }
    public float ScrollTop { get; set; }
}