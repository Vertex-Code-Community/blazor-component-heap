namespace Bch.Components.Table.Models;

public class ButtonData<TItem> where TItem : class
{
    public required TItem Data { get; set; }
    public required string ButtonName { get; set; }
}
