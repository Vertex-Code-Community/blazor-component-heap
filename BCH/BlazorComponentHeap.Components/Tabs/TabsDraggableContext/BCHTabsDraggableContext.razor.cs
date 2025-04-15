using BlazorComponentHeap.Components.Models.Tab;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Components.Tabs.TabsDraggableContext;

public partial class BCHTabsDraggableContext<TItem> : ComponentBase where TItem : class
{
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    internal readonly TabsDraggableContextModel<TItem> DraggableContext = new();

    protected override void OnAfterRender(bool firstRender)
    {
        // Console.WriteLine("BCHTabsDraggableContext OnAfterRender");
    }
}