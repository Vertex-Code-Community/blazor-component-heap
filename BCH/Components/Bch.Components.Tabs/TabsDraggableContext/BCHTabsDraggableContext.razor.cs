using Bch.Components.Tabs.Models;
using Microsoft.AspNetCore.Components;

namespace Bch.Components.Tabs.TabsDraggableContext;

public partial class BCHTabsDraggableContext<TItem> : ComponentBase where TItem : class
{
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    internal readonly TabsDraggableContextModel<TItem> DraggableContext = new();

    protected override void OnAfterRender(bool firstRender)
    {
        // Console.WriteLine("BCHTabsDraggableContext OnAfterRender");
    }
}