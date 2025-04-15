namespace BlazorComponentHeap.Shared.Models.Table;

public class ButtonData<TItem> where TItem : class
{
    public TItem Data { get; set; }
    public string ButtonName { get; set; }
}
