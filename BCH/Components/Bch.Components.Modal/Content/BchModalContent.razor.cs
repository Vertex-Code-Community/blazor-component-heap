using Microsoft.AspNetCore.Components;
using Bch.Components.Modal.Models;
using Bch.Components.Modal.Services;

namespace Bch.Components.Modal.Content;

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