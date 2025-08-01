using Microsoft.AspNetCore.Components;
using BlazorComponentHeap.DomInterop.Services;
using BlazorComponentHeap.ExpandMenu.ExpandMenuContainer;

namespace BlazorComponentHeap.ExpandMenu.ExpandMenuComponent;

public partial class BCHExpandMenu : IDisposable
{
    [Inject] public required IDomInteropService DomInteropService { get; set; }

    [CascadingParameter(Name = "BCHExpandMenuContainer")] public BCHExpandMenuContainer OwnerContainer { get; set; } = null!;
    
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string Description { get; set; } = string.Empty;
    [Parameter] public bool Disabled { get; set; }

    private readonly string _descriptionId = $"_id_{Guid.NewGuid()}";
    private double _heightDescription = 0;

    protected override void OnInitialized()
    {
        OwnerContainer.AddExpandMenu(this);
        // IJSUtilsService.OnResize += OnResizeAsync;
    }
    
    public void Dispose()
    {
        // IJSUtilsService.OnResize -= OnResizeAsync;
    }

    private async Task OnResizeAsync()
    {
        var boundingClientRect = await DomInteropService.GetBoundingClientRectAsync(_descriptionId);
        if (boundingClientRect is null) return;
        
        _heightDescription = boundingClientRect.OffsetHeight;
        StateHasChanged();
    }

    private async Task OnClick()
    {
        await OnResizeAsync();
        if (Disabled) return;

        OwnerContainer.SelectButton(this);
    }

    public void Update() => StateHasChanged();
}
