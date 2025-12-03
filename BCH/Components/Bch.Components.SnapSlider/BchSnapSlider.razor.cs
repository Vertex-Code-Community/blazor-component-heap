using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bch.Components.SnapSlider;

public partial class BchSnapSlider<TItem> : ComponentBase, IAsyncDisposable
{
    [Inject] public required IJSRuntime JsRuntime { get; set; }

    [Parameter] public required RenderFragment<TItem> ItemTemplate { get; set; }
    [Parameter] public Func<int, TItem> OnItem { get; set; } = _ => default!;

    [Parameter]
    public int ScrollIndex
    {
        get => _scrollIndex;
        set
        {
            if (value == _scrollIndex) return;
            _scrollIndex = value;
            ScrollIndexChanged.InvokeAsync(value);
        }
    }

    [Parameter] public EventCallback<int> ScrollIndexChanged { get; set; }
    [Parameter] public int DefaultLeftShift { get; set; } = 2000;

    private readonly string _id = $"_id_{Guid.NewGuid()}";
    private readonly string _scrollerId = $"_id_{Guid.NewGuid()}";

    private int _scrollIndex = 0;
    private bool _firstRendered = false;
    private readonly NumberFormatInfo _nfWithDot = new() { NumberDecimalSeparator = ".", NumberDecimalDigits = 14 };

    private DotNetObjectReference<BchSnapSlider<TItem>>? _dotNetObjectReference;

    protected override async Task OnInitializedAsync()
    {
        _dotNetObjectReference = DotNetObjectReference.Create(this);
        await JsRuntime.InvokeVoidAsync("bchRegisterSnapFeedbackRef", _dotNetObjectReference,
            _scrollerId, DefaultLeftShift);
    }

    public async ValueTask DisposeAsync()
    {
        await JsRuntime.InvokeVoidAsync("bchReleaseSnapFeedbackRef", _scrollerId);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        
        await Task.Yield();
        _firstRendered = true;
        StateHasChanged();
    }

    [JSInvokable]
    public Task OnNextCalledFromScrollListenerAsync(int index)
    {
        OnNextClicked(index);
        return Task.CompletedTask;
    }

    private void OnNextClicked(int k)
    {
        ScrollIndex += k;
        StateHasChanged();
    }
}