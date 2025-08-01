using System.Globalization;
using Microsoft.AspNetCore.Components;
using BlazorComponentHeap.DomInterop.Services;
using BlazorComponentHeap.Tabs.Models;

namespace BlazorComponentHeap.Tabs.TabPanelScroller;

public partial class BCHTabPanelScroller<TItem> : ComponentBase, IDisposable where TItem : class
{
    [Inject] public required IDomInteropService DomInteropService { get; set; }
    
    [Parameter] public int Gap { get; set; }
    [Parameter] public int TabHeight { get; set; }
    [Parameter] public int ScrollTabWidth { get; set; }
    [Parameter] public bool ScrollToSelected { get; set; }
    [Parameter] public TabsDraggableContextModel<TItem> DraggableContext { get; set; } = null!;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public Func<TItem, int> TabWidthPredicate { get; set; } = x => 100;
    [Parameter] public TItem? Selected { get; set; }
    [Parameter] public List<TItem> Items { get; set; } = new();
    
    private readonly string _panelContainerId = $"_id_{Guid.NewGuid()}";
    private readonly string _panelScrollerId = $"_id_{Guid.NewGuid()}";
    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };

    private float _scrollOffset = 0;
    private string _scrollStr = "0";
    private bool _showLeft = false;
    private bool _showRight = false;
    private TItem? _prevSelected = null;

    // protected override void OnInitialized()
    // {
    //     IJSUtilsService.OnResize += UpdateScrollAsync;
    //     IJSUtilsService.OnResize += UpdateControlButtonsAsync;
    // }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            DraggableContext.OnUpdateScroller += UpdateAsync;
            DraggableContext.OnRerenderScroller += StateHasChanged;
        } 
        // Console.WriteLine("BCHTabPanelScroller OnAfterRender");
        await UpdateAsync(); // TODO: improve ???
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_prevSelected != Selected && ScrollToSelected)
        {
            // Console.WriteLine("Selected changed");
            _prevSelected = Selected;
            if (Selected != null) await ScrollAsync(Selected);
        }
    }

    public void Dispose()
    {
        // IJSUtilsService.OnResize -= UpdateScrollAsync;
        // IJSUtilsService.OnResize -= UpdateControlButtonsAsync;
        
        DraggableContext.OnUpdateScroller -= UpdateAsync;
        DraggableContext.OnRerenderScroller -= StateHasChanged;
    }

    private async Task UpdateAsync()
    {
        await UpdateControlButtonsAsync();
        await UpdateScrollAsync();
    }
    
    private async Task UpdateScrollAsync() => await UpdateScrollAsync(0);
    private async Task UpdateScrollAsync(float offset)
    {
        var panelRect = await DomInteropService.GetBoundingClientRectAsync(_panelContainerId);
        var scrollerRect = await DomInteropService.GetBoundingClientRectAsync(_panelScrollerId);

        if (panelRect is null || scrollerRect is null) return;

        var rightValue = Math.Min((float)(panelRect.Width - scrollerRect.Width), 0.0f);
        var newScrollOffset = Math.Clamp(_scrollOffset + offset, rightValue, 0.0f);

        if (newScrollOffset != _scrollOffset)
        {
            _scrollOffset = newScrollOffset;
            _scrollStr = _scrollOffset.ToString(_nF);

            _showLeft = _scrollOffset != 0.0f;
            _showRight = _scrollOffset != rightValue;

            // LeftReached = _showLeft;
            // RightReached = _showRight;

            StateHasChanged();
        }
    }
    
    private async Task UpdateControlButtonsAsync()
    {
        var containerRect = await DomInteropService.GetBoundingClientRectAsync(_panelContainerId);
        var draggableRect = await DomInteropService.GetBoundingClientRectAsync(_panelScrollerId);

        if (containerRect == null! || draggableRect == null!) return;

        var rightValue = Math.Min((float)(containerRect.Width - draggableRect.Width), 0.0f);

        var oldShowLeft = _showLeft;
        var oldShowRight = _showRight;
        
        _showLeft = _scrollOffset != 0.0f;
        _showRight = _scrollOffset != rightValue;
        
        // LeftReached = _showLeft;
        // RightReached = _showRight || Math.Abs(_scrollOffset) < containerRect.Width;
        
        if (oldShowLeft != _showLeft || oldShowRight != _showRight)
        {
            StateHasChanged();
        }
    }
    
    private async Task OnLeftClickAsync() => await UpdateScrollAsync(ScrollTabWidth);

    private async Task OnRightClickAsync() => await UpdateScrollAsync(-ScrollTabWidth);
    
    private async Task ScrollAsync(TItem item)
    {
        var containerRect = await DomInteropService.GetBoundingClientRectAsync(_panelContainerId);
        var draggableRect = await DomInteropService.GetBoundingClientRectAsync(_panelScrollerId);

        if (containerRect is null || draggableRect is null) return;

        var itemIndex = Items.IndexOf(item);
        var itemWidth = TabWidthPredicate.Invoke(item);
        var offsetLeft = 0;

        for (var i = 0; i < itemIndex; i ++)
        {
            offsetLeft += TabWidthPredicate.Invoke(Items[i]) + Gap;
        }
        
        var rightValue = Math.Min((float)(containerRect.Width - draggableRect.Width), 0.0f);
        var newScrollOffset = Math.Clamp(
            -(offsetLeft - (float)(containerRect.Width * 0.5f) + itemWidth * 0.5f),
            rightValue,
            0.0f
        );
    
        if (newScrollOffset != _scrollOffset)
        {
            _scrollOffset = newScrollOffset;
            _scrollStr = _scrollOffset.ToString(_nF);
    
            _showLeft = _scrollOffset != 0.0f;
            _showRight = _scrollOffset != rightValue;
    
            // LeftReached = _showLeft;
            // RightReached = _showRight;
    
            StateHasChanged();
        }
    }
}