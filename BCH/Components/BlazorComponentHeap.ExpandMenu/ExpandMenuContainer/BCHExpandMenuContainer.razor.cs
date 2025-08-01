using Microsoft.AspNetCore.Components;
using BlazorComponentHeap.ExpandMenu.ExpandMenuComponent;

namespace BlazorComponentHeap.ExpandMenu.ExpandMenuContainer;

public partial class BCHExpandMenuContainer
{
    [Parameter] public required RenderFragment ChildContent { get; set; }

    private List<BCHExpandMenu> _expandMenus = new();

    internal BCHExpandMenu SelectedMenu { get; set; } = null!;

    internal void AddExpandMenu(BCHExpandMenu expandMenu)
    {
        _expandMenus.Add(expandMenu);
    }

    internal void SelectButton(BCHExpandMenu expandMenu)
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
