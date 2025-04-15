using BlazorComponentHeap.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Components.Core.SubscriptionEnabler;

public partial class BCHSubscriptionEnabler : IDisposable
{
    [Inject] private IUpdateCheckerService UpdateCheckerService { get; set; } = null!;
    
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool ShowNotification { get; set; } = true;

    protected override void OnInitialized()
    {
        UpdateCheckerService.OnUpdate += StateHasChanged;
    }

    public void Dispose()
    {
        UpdateCheckerService.OnUpdate -= StateHasChanged;
    }
}