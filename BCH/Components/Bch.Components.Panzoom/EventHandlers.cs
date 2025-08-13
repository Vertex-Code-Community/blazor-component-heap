using Bch.Modules.GlobalEvents.Events;
using Microsoft.AspNetCore.Components;

namespace Bch.Components.Panzoom;

[EventHandler("onbchpanzoomscroll", typeof(BchWheelEventArgs), true, true)]
[EventHandler("onbchpanzoomtouchstart", typeof(BchTouchEventArgs), true, true)]
[EventHandler("onbchpanzoomtouchmove", typeof(BchTouchEventArgs), true, true)]
[EventHandler("onbchpanzoomtouchend", typeof(BchTouchEventArgs), true, true)]
public static class EventHandlers
{
}


