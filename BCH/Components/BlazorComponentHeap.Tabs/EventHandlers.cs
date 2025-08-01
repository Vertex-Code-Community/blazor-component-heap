using BlazorComponentHeap.GlobalEvents.Events;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Tabs;

[EventHandler("onexttouchstart", typeof(BchTouchEventArgs), true, true)]
[EventHandler("onextmousedown", typeof(BchMouseEventArgs), true, true)]
public static class EventHandlers
{
}