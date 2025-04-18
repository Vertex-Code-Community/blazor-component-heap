using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Tabs.Tabs.TabPanel.DefaultTab;

public partial class BCHDefaultTab<TItem> : ComponentBase where TItem : class
{
    [Parameter] public TItem Item { get; set; } = null!;
    [Parameter] public TItem SelectedItem { get; set; } = null!;
    [Parameter] public bool ShowCloseButton { get; set; }
    [Parameter] public EventCallback<TItem> OnClose { get; set; }
    [Parameter] public Func<TItem, string>? DefaultTabText { get; set; }

    private bool IsSelected => Item == SelectedItem;
    
    private readonly string _iconImage = "_content/BlazorComponentHeap.Core/bch-img/tabs/default-icon/default-tab.svg";
    private readonly string _selectedIconImage = "_content/BlazorComponentHeap.Core/bch-img/tabs/default-icon/default-tab-selected.svg";
    private readonly string _closeIcon = "_content/BlazorComponentHeap.Core/bch-img/tabs/close-tab.svg";
    private readonly string _closeIconSelected = "_content/BlazorComponentHeap.Core/bch-img/tabs/close-tab-selected.svg";

    private Func<TItem, string> _defaultTabText = x => $"{x}";
    
    protected override void OnInitialized()
    {
        if (DefaultTabText is not null)
        {
            _defaultTabText = DefaultTabText;
        }
    }

    private async Task OnCloseClickedAsync()
    {
        if (ShowCloseButton) await OnClose.InvokeAsync(Item);
    }
}