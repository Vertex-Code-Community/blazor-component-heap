namespace Bch.Integration.Pages.Pages.Select;

public partial class SelectPage
{
    class SelectItem
    {
        public string Name { get; set; } = string.Empty;
        public int Type { get; set; }
    }

    private List<string> _emptyList = new List<string>();
    private readonly List<int> _itemsInts = Enumerable.Range(0, 20).ToList();
    private readonly List<int?> _itemsInts2 = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20};
    private readonly List<string> _items = new List<string>
    {
        "Item 1 tt",
        "Item 2",
        "Item 2 s",
        "Item 3 tt",
        "Item 4",
        "Item 4 s",
        "Item 5 tt",
        "Item 6",
        "Item 7 tt",
        "Item 7 s",
        "Item 8",
        "Item 9 tt",
        "Item 9 s",
        "Item 10",
        "Item 10 s",
        "Item 11",
        "Item 12",
        "Item 13 tt",
        "Item 13 s",
        "Item 14",
        "Item 15 tt",
        "Item 16",
        "Item 17 tt",
        "Item 18",
        "Item 18 s",
        "Item 19 tt",
        "Item 20",
        "Item 20 s"
    };

    private List<string> _selectedItems = new();

    private bool _isOpened = false;
    private string? _selected;

    protected override void OnAfterRender(bool firstRender)
    {
        foreach (var selected in _selectedItems)
        {
            Console.Write($"{selected} ");
        }

        // Console.WriteLine("_ OnAfterRender _");
    }
}
