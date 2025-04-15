using BlazorComponentHeap.TestApp.Routing.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.TestApp.Pages;

public partial class Index
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    // [Inject] private FlexibleNavigationManager NavigationManager { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    private async Task NavigateToAsync(string url)
    {
        await JsRuntime.InvokeVoidAsync("navigateToWithoutSaving", url);
    }
}
