using Microsoft.AspNetCore.Components;
using Bch.Components.Modal.Services;

namespace Bch.Components.Modal.Root;

public partial class BchRootModal : IDisposable
{
    [Inject] public required IModalService ModalService { get; set; }

    protected override void OnInitialized()
    {
        ModalService.OnUpdate += StateHasChanged;
    }

    public void Dispose()
    {
        ModalService.OnUpdate -= StateHasChanged;
    }
}
