using Microsoft.AspNetCore.Components;
using Bch.Modules.GlobalEvents.Events;

namespace Bch.Components.Tabs;

[EventHandler("onexttouchstart", typeof(BchTouchEventArgs), true, true)]
[EventHandler("onextmousedown", typeof(BchMouseEventArgs), true, true)]
public static class EventHandlers
{
}