using Microsoft.AspNetCore.Components;
using Bch.Modules.GlobalEvents.Events;

namespace Bch.Components.Tabs;

[EventHandler("onbchtabstouchstart", typeof(BchTouchEventArgs), true, true)]
[EventHandler("onbchtabsmousedown", typeof(BchMouseEventArgs), true, true)]
public static class EventHandlers
{
}