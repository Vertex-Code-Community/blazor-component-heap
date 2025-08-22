using Bch.Components.ExpandMenu.ExpandMenuComponent;
using Microsoft.AspNetCore.Components;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.ExpandMenu.ExpandMenuContainer;

public partial class BchExpandMenuContainer
{
    [Parameter] public required RenderFragment ChildContent { get; set; }
    
    [Parameter] public BchTheme? Theme { get; set; }
    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }

    private List<BchExpandMenu> _expandMenus = new();
    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
    private string GetThemeCssClass() => EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;

    internal BchExpandMenu SelectedMenu { get; set; } = null!;

    internal void AddExpandMenu(BchExpandMenu expandMenu)
    {
        _expandMenus.Add(expandMenu);
    }

    internal void SelectButton(BchExpandMenu expandMenu)
    {
        if (SelectedMenu == expandMenu)
        {
            SelectedMenu = null!;
            expandMenu.Update();
            return;
        }

        SelectedMenu = expandMenu;

        foreach (var exMenu in _expandMenus)
        {
            exMenu.Update();
        }
    }
}
