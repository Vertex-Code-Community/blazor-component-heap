namespace Bch.Modules.GlobalEvents.Models;

public class ElementParameters
{
    public string Id { get; set; } = string.Empty;
    public string TagName { get; set; } = string.Empty;
    public string ClassList { get; set; } = string.Empty;
    public Dictionary<string, string> StyleMap { get; set; } = new();
    public float X { get; set; }
    public float Y { get; set; }
    public float ClientWidth { get; set; }
    public float ClientHeight { get; set; }
    public float ScrollTop { get; set; }
}