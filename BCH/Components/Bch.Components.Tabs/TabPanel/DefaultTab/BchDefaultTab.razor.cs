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
    
    private string IconImage => EffectiveTheme switch
    {
        BchTheme.LightGreen => "_content/Bch.Components.Tabs/img/default-icon/default-tab.svg",
        BchTheme.Light => "_content/Bch.Components.Tabs/img/default-icon/default-tab-light.svg",
        BchTheme.Dark => "_content/Bch.Components.Tabs/img/default-icon/default-tab-dark.svg",
        _ => "_content/Bch.Components.Tabs/img/default-icon/default-tab.svg"
    };
    private string SelectedIconImage => EffectiveTheme switch
    {
        BchTheme.LightGreen => "_content/Bch.Components.Tabs/img/default-icon/default-tab-selected.svg",
        BchTheme.Light => "_content/Bch.Components.Tabs/img/default-icon/default-tab-selected-light.svg",
        BchTheme.Dark => "_content/Bch.Components.Tabs/img/default-icon/default-tab-selected-dark.svg",
        _ => "_content/Bch.Components.Tabs/img/default-icon/default-tab-selected.svg"
    };
    private readonly string _closeIcon = "_content/Bch.Components.Tabs/img/close-tab.svg";
    private readonly string _closeIconSelected = "_content/Bch.Components.Tabs/img/close-tab-selected.svg";

    private Func<TItem, string> _defaultTabText = x => $"{x}";
    
    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
    private readonly string _cssKey = $"_cssKey_{Guid.NewGuid()}";
    private string GetThemeCssClass()
    {
        var themeSpecified = Theme ?? ThemeCascading;
        var themeClass = EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;
        return themeClass + (themeSpecified is null ? " bch-no-theme-specified" : "");
    }
    
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