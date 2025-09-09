using Microsoft.AspNetCore.Components;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.Tabs.TabDraggingContent;

public partial class BchTabDraggingContent<TItem> : ComponentBase where TItem : class
{
    [Parameter] public int TabHeight { get; set; }
    [Parameter] public TItem Item { get; set; } = null!;
    [Parameter] public required RenderFragment<TItem> TabTemplate { get; set; }
    [Parameter] public required RenderFragment<TItem> ContentTemplate { get; set; }
    [Parameter] public Func<TItem, int>? TabWidthPredicate { get; set; } = x => 100;
    [Parameter] public BchTheme? Theme { get; set; }
    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }

    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
    private string GetThemeCssClass()
    {
        var themeSpecified = Theme ?? ThemeCascading;
        var themeClass = EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;
        return themeClass + (themeSpecified is null ? " bch-no-theme-specified" : "");
    }
}