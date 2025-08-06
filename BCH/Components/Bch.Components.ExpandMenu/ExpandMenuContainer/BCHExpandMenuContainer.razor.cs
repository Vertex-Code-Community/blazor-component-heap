using Bch.Components.ExpandMenu.ExpandMenuComponent;
using Microsoft.AspNetCore.Components;

namespace Bch.Components.ExpandMenu.ExpandMenuContainer;

public partial class BchExpandMenuContainer
{
    [Parameter] public required RenderFragment ChildContent { get; set; }

    private List<BchExpandMenu> _expandMenus = new();

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
