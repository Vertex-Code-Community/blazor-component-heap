using BlazorComponentHeap.Core.Models;
using BlazorComponentHeap.Core.Services.Interfaces;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.Core.Services;

public class ModalService : IModalService
{
    public IReadOnlyList<ModalModel> Modals { get; }
    public event Action? OnUpdate;
    public event Action<ModalModel>? OnOverlayClicked = null!;

    private readonly IJSRuntime _jsRuntime;
    // private readonly Action _onAddRootModal;
    private readonly List<ModalModel> _modals = new();

    public ModalService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        Modals = _modals;

        // _onAddRootModal = async () => { await AddRootModalDynamicallyAsync(); };
        // _onAddRootModal.Invoke();
    }

    // private async Task AddRootModalDynamicallyAsync()
    // {
    //     await _jsRuntime.InvokeVoidAsync("bchAddRootModal");
    // }

    public void Open(ModalModel modalModel)
    {
        if (_modals.Contains(modalModel)) return;
        
        _modals.Add(modalModel);
        OnUpdate?.Invoke();
    }

    public void Close(ModalModel modalModel)
    {
        if (!_modals.Contains(modalModel)) return;
        
        _modals.Remove(modalModel);
        OnUpdate?.Invoke();
    }

    public void FireOverlayClicked(ModalModel modalModel)
    {
        OnOverlayClicked?.Invoke(modalModel);
    }
}
