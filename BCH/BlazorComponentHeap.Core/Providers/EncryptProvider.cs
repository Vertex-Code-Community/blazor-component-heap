using Microsoft.JSInterop;
using System.Text;
using System.Text.Json;

namespace BlazorComponentHeap.Core.Providers;

internal class EncryptProvider
{
    private readonly IJSRuntime _jsRuntime;

    private int[] _hiddenKey = new int[] { 0, 45, 6, 3, 8, 5, 8, 7, 89, 7, 10, 21, 12, 34, 12, 1 }; // should be calculated

    public EncryptProvider(IJSRuntime jSRuntime)
    {
        _jsRuntime = jSRuntime;
    }

    internal async Task<string> DecryptTextAsync(string key, string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        try
        {
            var keyByteArray = UTF8Encoding.UTF8.GetBytes(key);
            var result = await _jsRuntime.InvokeAsync<string>("bchDecryptText", input, _hiddenKey);

            return result;
        }
        catch (Exception e)
        {
            return string.Empty;
        }
    }

    internal async Task<string> EncryptTextAsync(string key, string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        try
        {
            var keyByteArray = UTF8Encoding.UTF8.GetBytes(key);
            var result = await _jsRuntime.InvokeAsync<string>("bchEncryptText", input, _hiddenKey);

            return result;
        }
        catch (Exception e)
        {
            return string.Empty;
        }
    }

    internal async Task<string> EncryptAsync<T>(string key, T input)
    {
        var str = JsonSerializer.Serialize(input);

        if (input == null)
            return string.Empty;
        try
        {
            return await _jsRuntime.InvokeAsync<string>("bchEncryptText", input, _hiddenKey);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    internal async Task<T> DecryptAsync<T>(string key, string input)
    {
        if (string.IsNullOrEmpty(input))
            return default(T)!;
        try
        {
            var keyByteArray = UTF8Encoding.UTF8.GetBytes(key);

            return await _jsRuntime.InvokeAsync<T>("bchDecryptText", input, _hiddenKey);
        }
        catch (Exception)
        {
            return default!;
        }
    }
}
