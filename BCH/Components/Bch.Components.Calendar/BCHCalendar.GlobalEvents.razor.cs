using Bch.Modules.GlobalEvents.Events;
using Bch.Modules.GlobalEvents.Models;

namespace Bch.Components.Calendar;

public partial class BchCalendar
{
    private Task SubscribeOnGlobalScrollAsync()
    {
        return GlobalEventsService.AddDocumentListenerAsync<BchScrollEventArgs>("scroll", 
            _subscriptionKey, OnWindowGlobalScrollAsync, subscribingContext: GlobalSubscribingContext.Window);
    }

    private Task UnsubscribeFromGlobalScrollAsync()
    {
        return GlobalEventsService.RemoveDocumentListenerAsync<BchScrollEventArgs>("scroll", _subscriptionKey);
    }
}