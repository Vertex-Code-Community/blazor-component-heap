using Microsoft.AspNetCore.Components;

namespace Bch.Components.Tabs.TabDraggingContent;

public partial class BchTabDraggingContent<TItem> : ComponentBase where TItem : class
{
    [Parameter] public int TabHeight { get; set; }
    [Parameter] public TItem Item { get; set; } = null!;
    [Parameter] public required RenderFragment<TItem> TabTemplate { get; set; }
    [Parameter] public required RenderFragment<TItem> ContentTemplate { get; set; }
    [Parameter] public Func<TItem, int>? TabWidthPredicate { get; set; } = x => 100;
}