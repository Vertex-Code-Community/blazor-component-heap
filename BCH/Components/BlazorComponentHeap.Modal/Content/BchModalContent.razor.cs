using Microsoft.AspNetCore.Components;
using BlazorComponentHeap.Modal.Models;
using BlazorComponentHeap.Modal.Services;

namespace BlazorComponentHeap.Modal.Content;

public partial class BchModalContent : IDisposable
{
    [Inject] public required IModalService ModalService { get; set; }

    [Parameter] public ModalModel ModalModel { get; set; } = null!;

    protected override void OnInitialized()
    {
        ModalModel.OnUpdate += StateHasChanged;
    }
    
    public void Dispose()
    {
        ModalModel.OnUpdate -= StateHasChanged;
    }
    
    private void OnOverlayClicked()
    {
        ModalService.FireOverlayClicked(ModalModel);
    }
    
    private bool IsCenter()
    {
        return string.IsNullOrWhiteSpace(ModalModel.X) || string.IsNullOrWhiteSpace(ModalModel.Y);
    }
}