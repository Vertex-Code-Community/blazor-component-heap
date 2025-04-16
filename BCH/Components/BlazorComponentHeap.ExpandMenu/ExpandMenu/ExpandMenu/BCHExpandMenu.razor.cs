using BlazorComponentHeap.Core.Services.Interfaces;
using BlazorComponentHeap.ExpandMenu.ExpandMenu.ExpandMenuContainer;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.ExpandMenu.ExpandMenu.ExpandMenu;

public partial class BCHExpandMenu : IDisposable
{
    [Inject] private IJSUtilsService JSUtilsService { get; set; }

    [CascadingParameter(Name = "BCHExpandMenuContainer")] public BCHExpandMenuContainer OwnerContainer { get; set; } = null!;
    
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string Description { get; set; } = string.Empty;
    [Parameter] public bool Disabled { get; set; }

    private string _descriptionId = $"_id_{Guid.NewGuid()}";
    private double _heightDescription = default(double);

    protected override void OnInitialized()
    {
        OwnerContainer.AddExpandMenu(this);
        IJSUtilsService.OnResize += OnResizeAsync;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            
        }
    }

    private async Task OnResizeAsync()
    {
        var boundingClientRect = await JSUtilsService.GetBoundingClientRectAsync(_descriptionId);
        _heightDescription = boundingClientRect.OffsetHeight;
        StateHasChanged();
    }

    private async Task OnClick()
    {
        await OnResizeAsync();

        if (Disabled)
        {
            return;
        }

        OwnerContainer.SelectButton(this);
    }

    public void Update() => StateHasChanged();

    public void Dispose()
    {
        IJSUtilsService.OnResize -= OnResizeAsync;
    }
}
