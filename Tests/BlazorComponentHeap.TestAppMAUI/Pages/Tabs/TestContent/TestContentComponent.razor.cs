namespace BlazorComponentHeap.TestAppMAUI.Pages.Tabs.TestContent;

public partial class TestContentComponent
{
    private readonly List<string> _items1 = Enumerable.Range(0, 5).Select(x => $"item {x}").ToList();
    private readonly List<string> _items2 = Enumerable.Range(0, 5).Select(x => $"ITEM {x}").ToList();
    private readonly List<string> _items3 = Enumerable.Range(0, 5).Select(x => $"ItEm {x}").ToList();
    private readonly List<string> _items4 = Enumerable.Range(0, 5).Select(x => $"iTeM {x}").ToList();

    private string _selected1 = string.Empty;
    private string _selected2 = string.Empty;
    private string _selected3 = string.Empty;
    private string _selected4 = string.Empty;

    protected override void OnAfterRender(bool firstRender)
    {
        // Console.WriteLine("TestContentComponent OnAfterRender");
    }
}
