using BlazorComponentHeap.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Modal.Modal.Root;

public partial class BCHRootModal : IDisposable
{
    [Inject] private IModalService ModalService { get; set; } = null!;

    protected override void OnInitialized()
    {
        ModalService.OnUpdate += StateHasChanged;
    }

    public void Dispose()
    {
        ModalService.OnUpdate -= StateHasChanged;
    }
}
