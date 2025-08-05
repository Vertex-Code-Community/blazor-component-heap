using Microsoft.AspNetCore.Components;

namespace Bch.Components.Tabs.TabDraggingContent;

public partial class BCHTabDraggingContent<TItem> : ComponentBase where TItem : class
{
    [Parameter] public int TabHeight { get; set; }
    [Parameter] public TItem Item { get; set; } = null!;
    [Parameter] public RenderFragment<TItem> TabTemplate { get; set; } = null!;
    [Parameter] public RenderFragment<TItem> ContentTemplate { get; set; } = null!;
    [Parameter] public Func<TItem, int>? TabWidthPredicate { get; set; } = x => 100;
}