using BlazorComponentHeap.Shared.Models.Events;
using BlazorComponentHeap.Shared.Models.Markup;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.Core.Services.Interfaces;

public interface IJSUtilsService
{
    Task<BoundingClientRect?> GetBoundingClientRectAsync(string id);
    // BoundingClientRect GetBoundingClientRect(string id);
    // void ScrollTo(string id, string x, string y, string behavior = "smooth"); // auto
    Task ScrollToAsync(string id, string x, string y, string behavior = "smooth"); // auto

    ValueTask FocusAsync(string elementId);

    Task AddDocumentListenerAsync<T>(string eventName, string key, Func<T, Task> callback,
        bool preventDefault = false,
        bool stopPropagation = false,
        bool passive = true);
    Task RemoveDocumentListenerAsync<T>(string eventName, string key);
    
    static event Func<Task> OnResize = null!;
    static event Func<ScrollEventArgs, Task> OnGlobalScroll = null!;

    [JSInvokable] static async Task OnBrowserResizeAsync()
    {
        if (OnResize != null!)
        {
            await OnResize.Invoke();
        }
    }
    
    [JSInvokable] static async Task OnBrowserGlobalScrollAsync(ScrollEventArgs model)
    {
        if (OnGlobalScroll != null!)
        {
            await OnGlobalScroll.Invoke(model);
        }
    }
}
