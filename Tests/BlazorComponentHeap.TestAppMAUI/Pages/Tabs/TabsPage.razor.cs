namespace BlazorComponentHeap.TestAppMAUI.Pages.Tabs;

public partial class TabsPage
{
    // private TabContextModel _context = new TabContextModel(2) { Orderable = true };

    private class TabModel
    {
        public string Name { get; set; } = string.Empty;
        public int Width { get; set; }
    }
    
    private readonly List<TabModel> _items = Enumerable.Range(0, 10)
        .Select(x => new TabModel
        {
            Name = $"Item {x}",
            Width = x % 2 == 0 ? 100 : 150
        })
        .ToList();
    
    private readonly List<string> _items1 = Enumerable.Range(0, 10).Select(x => $"item {x}").ToList();
    private readonly List<string> _items2 = Enumerable.Range(0, 10).Select(x => $"ITEM {x}").ToList();

    private string _selected1 = string.Empty;
    private string _selected2 = string.Empty;
    
    protected override void OnAfterRender(bool firstRender)
    {
        // Console.WriteLine("TestPage OnAfterRender");
    }

    protected override void OnInitialized()
    {
        // for (int i = 0; i < 2; i++)
        // {
        //     for (int j = 0; j < 10; j++)
        //     {
        //         _context.TabPanels[i].TabModels.Add(new TabModel
        //         {
        //             Type = $"Tab {i} {j}",
        //             Name = $"Tab {i} {j}",
        //             Width = j % 2 == 0 ? 118 : 118,
        //             Height = 35,
        //             Closable = true,
        //             IconImage = "_content/BlazorComponentHeap.Components/img/tabs/default-icon/default-tab.svg",
        //             SelectedIconImage = "_content/BlazorComponentHeap.Components/img/tabs/default-icon/default-tab-selected.svg"
        //         });
        //     }
        // }
        //
        // _context.TabPanels[0].TabModels[0].Type = "additional-content";
        // _context.TabPanels[1].TabModels[0].Type = "additional-content";
    }
}
