using BlazorComponentHeap.GlobalEvents.Models;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.GlobalEvents.Services;

internal class GlobalEventsService : IGlobalEventsService
{
    private readonly IJSRuntime _jsRuntime;

    private class DocumentListenerHolder<T>
    {
        public Dictionary<string, Func<T, Task>> Functions { get; set; } = new();
        public DotNetObjectReference<DocumentListenerHolder<T>> DotNetRef { get; }
        
        public DocumentListenerHolder()
        {
            DotNetRef = DotNetObjectReference.Create(this);
        }

        [JSInvokable]
        public void Callback(T e)
        {
            foreach (var keyValue in Functions)
            {
                keyValue.Value.Invoke(e);
            }
        }
    }
    
    private readonly Dictionary<string, object> _docListeners = new();
    
    public GlobalEventsService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task AddDocumentListenerAsync<T>(string eventName, string key, Func<T, Task> callback, 
        bool preventDefault = false,
        bool stopPropagation = false,
        bool passive = true,
        GlobalSubscribingContext subscribingContext = GlobalSubscribingContext.Document)
    {
        var eventKey = $"{eventName}{typeof(T).FullName}";
        var subscriberKey = $"{eventName}{key}";
        
        if (!(_docListeners.TryGetValue(eventKey, out var docListenerObj) 
            && docListenerObj is DocumentListenerHolder<T> docListener))
        {
            docListener = new DocumentListenerHolder<T>();
            docListener.Functions.Add(subscriberKey, callback);
            _docListeners.Add(eventKey, docListener);

            var jsListenerKey = eventKey;
            await _jsRuntime.InvokeVoidAsync("bchAddDocumentListener", jsListenerKey, eventName, 
                docListener.DotNetRef, "Callback", (int) subscribingContext, preventDefault, stopPropagation, passive);
            
            return;
        }
        
        docListener.Functions.TryAdd(subscriberKey, callback);
    }

    public async Task RemoveDocumentListenerAsync<T>(string eventName, string key)
    {
        var eventKey = $"{eventName}{typeof(T).FullName}";
        var subscriberKey = $"{eventName}{key}";
        
        if (_docListeners.TryGetValue(eventKey, out var docListenerObj) 
              && docListenerObj is DocumentListenerHolder<T> docListener)
        {
            docListener.Functions.Remove(subscriberKey);

            if (docListener.Functions.Count == 0)
            {
                _docListeners.Remove(eventKey);
                var jsListenerKey = eventKey;

                // https://stackoverflow.com/questions/72488563/blazor-server-side-application-throwing-system-invalidoperationexception-javas
                try
                {
                    await _jsRuntime.InvokeVoidAsync("bchRemoveDocumentListener", jsListenerKey, eventName);
                }
                catch (JSDisconnectedException ex)
                {
                    // Ignore
                }
            }
        }
    }
}