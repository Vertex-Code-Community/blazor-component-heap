using Bch.Modules.GlobalEvents.Models;

namespace Bch.Modules.GlobalEvents.Services;

public interface IGlobalEventsService
{
    Task AddDocumentListenerAsync<T>(string eventName, string key, Func<T, Task> callback,
        bool preventDefault = false,
        bool stopPropagation = false,
        bool passive = true,
        GlobalSubscribingContext subscribingContext = GlobalSubscribingContext.Document);
    Task RemoveDocumentListenerAsync<T>(string eventName, string key);
}