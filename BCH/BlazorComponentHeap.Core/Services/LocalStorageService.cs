using BlazorComponentHeap.Core.Services.Interfaces;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.Core.Services;

public class LocalStorageService : ILocalStorageService
{
    private readonly IJSRuntime _jsRuntime = null!;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<T> GetItemAsync<T>(string key)
    {
        return await _jsRuntime.InvokeAsync<T>("window.localStorage.getItem", key);
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        await _jsRuntime.InvokeVoidAsync("window.localStorage.setItem", key, value);
    }
}
