using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bch.Integration.Pages.Pages;

public partial class Index : ComponentBase
{
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    // [Inject] private FlexibleNavigationManager NavigationManager { get; set; } = null!;
    [Inject] public required NavigationManager NavigationManager { get; set; }

    private async Task NavigateToAsync(string url)
    {
        await JsRuntime.InvokeVoidAsync("navigateToWithoutSaving", url);
    }
}
