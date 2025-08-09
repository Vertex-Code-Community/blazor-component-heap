using Bch.Components.Tabs.Models;
using Microsoft.AspNetCore.Components;

namespace Bch.Components.Tabs.TabsDraggableContext;

public partial class BchTabsDraggableContext<TItem> : ComponentBase where TItem : class
{
    [Parameter] public required RenderFragment ChildContent { get; set; }
    internal readonly TabsDraggableContextModel<TItem> DraggableContext = new();

    protected override void OnAfterRender(bool firstRender)
    {
        // Console.WriteLine("BCHTabsDraggableContext OnAfterRender");
    }
}