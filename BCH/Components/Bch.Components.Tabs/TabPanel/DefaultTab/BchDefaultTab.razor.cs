using Microsoft.AspNetCore.Components;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.Tabs.TabPanel.DefaultTab;

public partial class BchDefaultTab<TItem> : ComponentBase where TItem : class
{
    [Parameter] public TItem Item { get; set; } = null!;
    [Parameter] public TItem SelectedItem { get; set; } = null!;
    [Parameter] public bool ShowCloseButton { get; set; }
    [Parameter] public EventCallback<TItem> OnClose { get; set; }
    [Parameter] public Func<TItem, string>? DefaultTabText { get; set; }
    [Parameter] public BchTheme? Theme { get; set; }
    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }

    private bool IsSelected => Item == SelectedItem;
    
    private readonly string _iconImage = "_content/Bch.Components.Tabs/img/default-icon/default-tab.svg";
    private readonly string _selectedIconImage = "_content/Bch.Components.Tabs/img/default-icon/default-tab-selected.svg";
    private readonly string _closeIcon = "_content/Bch.Components.Tabs/img/close-tab.svg";
    private readonly string _closeIconSelected = "_content/Bch.Components.Tabs/img/close-tab-selected.svg";

    private Func<TItem, string> _defaultTabText = x => $"{x}";
    
    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
    private string GetThemeCssClass() => EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;
    
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