using BlazorComponentHeap.Core.Models.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorComponentHeap.Components;

// https://asp.net-hacker.rocks/2021/05/04/aspnetcore6-07-custom-event-arguments.html
// https://stackoverflow.com/questions/63265941/blazor-3-1-nested-onmouseover-events

[EventHandler("onextscroll", typeof(ScrollEventArgs), true, true)]
[EventHandler("onextmousewheel", typeof(ExtWheelEventArgs), true, true)]
[EventHandler("onmouseleave", typeof(MouseEventArgs), true, true)]

[EventHandler("onexttouchstart", typeof(ExtTouchEventArgs), true, true)]
[EventHandler("onextmousedown", typeof(ExtMouseEventArgs), true, true)]
public static class EventHandlers
{
}
