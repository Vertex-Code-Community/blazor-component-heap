using BlazorComponentHeap.GlobalEvents.Events;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Table;

[EventHandler("onbchtablescroll", typeof(BchScrollEventArgs), true, true)]
public static class EventHandlers
{
}
