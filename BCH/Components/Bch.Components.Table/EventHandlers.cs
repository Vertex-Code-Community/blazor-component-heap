using Bch.Modules.GlobalEvents.Events;
using Microsoft.AspNetCore.Components;

namespace Bch.Components.Table;

[EventHandler("onbchtablescroll", typeof(BchScrollEventArgs), true, true)]
public static class EventHandlers
{
}
