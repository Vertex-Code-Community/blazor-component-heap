using Bch.Components.Tabs.TabPanel;
using Bch.Modules.GlobalEvents.Models;
using Bch.Modules.Maths.Models;

namespace Bch.Components.Tabs.Models;

public class TabsDraggableContextModel<TItem> where TItem : class
{
    internal string ContextIdentifier { get; } = $"_cls_{Guid.NewGuid()}";
    internal TItem? DraggingItem { get; set; }
    internal Vec2 DraggingBounds { get; set; } = new();
    internal Vec2 DraggingPosition { get; set; } = new();
    internal Vec2 StartOffset { get; set; } = new();
    internal List<TItem>? Items { get; set; }
    internal BchTabPanel<TItem>? TabPanelInstance { get; set; }
    internal Action? OnUpdate { get; set; }
    internal Func<Task>? OnUpdateScroller { get; set; }
    internal Action? OnRerenderScroller { get; set; }
    internal Action<float, float, List<ElementParameters>>? OnMove { get; set; }
}