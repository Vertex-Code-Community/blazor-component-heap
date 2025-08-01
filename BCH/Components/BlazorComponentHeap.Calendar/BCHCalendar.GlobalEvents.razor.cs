using BlazorComponentHeap.GlobalEvents.Events;
using BlazorComponentHeap.GlobalEvents.Models;

namespace BlazorComponentHeap.Calendar;

public partial class BCHCalendar
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