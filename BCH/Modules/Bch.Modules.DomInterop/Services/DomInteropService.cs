using Bch.Modules.DomInterop.Models;
using Microsoft.JSInterop;

namespace Bch.Modules.DomInterop.Services;

internal class DomInteropService : IDomInteropService
{
    private readonly IJSRuntime _jsRuntime;
    
    public DomInteropService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public BoundingClientRect? GetBoundingClientRect(string id)
    {
        var jsInProgressRuntime = _jsRuntime as IJSInProcessRuntime;
        return jsInProgressRuntime?.Invoke<BoundingClientRect>("bchGetBoundingClientRectById", id);
    }

    public async Task<BoundingClientRect?> GetBoundingClientRectAsync(string id)
    {
        return await _jsRuntime.InvokeAsync<BoundingClientRect>("bchGetBoundingClientRectById", id);
    }

    public async Task ScrollToAsync(string id, string x, string y, string behavior = "smooth")
    {
        await _jsRuntime.InvokeVoidAsync("bchScrollElementTo", id, x, y, behavior);
    }

    public ValueTask FocusAsync(string elementId)
    {
        return _jsRuntime.InvokeVoidAsync("bchFocusElement", elementId);
    }

    public async Task<WindowSize> GetWindowSizeAsync()
    {
        return await _jsRuntime.InvokeAsync<WindowSize>("bchGetWindowSize");
    }
}