using System.Globalization;
using Microsoft.AspNetCore.Components;
using Bch.Modules.Files;
using Bch.Modules.GlobalEvents.Events;
using Bch.Modules.GlobalEvents.Services;
using Bch.Modules.Maths.Models;

namespace Bch.Components.InputFile.FilesList;

public partial class FilesListComponent  : IAsyncDisposable
{
    [Inject] public required IGlobalEventsService GlobalEventsService { get; set; }
    
    [Parameter] public required List<BchBrowserFile> Files { get; set; }
    [Parameter] public required Vec2 Position { get; set; }
    [Parameter] public required string Width { get; set; }
    [Parameter] public required string ContainerId { get; set; }
    
    [Parameter] public EventCallback<bool> ShowChanged { get; set; }
    [Parameter] public bool Show
    {
        get => _show;
        set
        {
            if (_show == value) return;
            _show = value;

            ShowChanged.InvokeAsync(value);
        }
    }
    
    private bool _show;
    
    private readonly string _contentContainerId = $"_id_{Guid.NewGuid()}";
    private readonly string _subscriptionKey = $"_key_{Guid.NewGuid()}";
    
    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };

    protected override Task OnInitializedAsync()
    {
        return GlobalEventsService.AddDocumentListenerAsync<BchMouseEventArgs>("mousedown", _subscriptionKey,
            OnDocumentMouseDownAsync);
    }

    public async ValueTask DisposeAsync()
    {
        await GlobalEventsService.RemoveDocumentListenerAsync<BchMouseEventArgs>("mousedown", _subscriptionKey);
    }
    
    private async Task OnDocumentMouseDownAsync(BchMouseEventArgs e)
    {
        var container = e.PathCoordinates
            .FirstOrDefault(x => x.Id == ContainerId || x.Id == _contentContainerId);

        if (container is not null) return; // inside of select
        
        _show = false;
        await ShowChanged.InvokeAsync(_show);
        StateHasChanged();
    }
}