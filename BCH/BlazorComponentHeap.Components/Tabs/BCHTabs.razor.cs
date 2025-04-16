using System.Globalization;
using BlazorComponentHeap.Components.Modal;
using BlazorComponentHeap.Components.Models.Tab;
using BlazorComponentHeap.Components.Tabs.TabsDraggableContext;
using BlazorComponentHeap.Core.Models.Math;
using BlazorComponentHeap.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Components.Tabs;

public partial class BCHTabs<TItem> : ComponentBase where TItem : class
{
    [Inject] private IJSUtilsService JsUtilsService { get; set; } = null!;
    
    [CascadingParameter(Name = $"BCHTabsDraggableContext{nameof(TItem)}")] 
    public BCHTabsDraggableContext<TItem>? OwnerContainer { get; set; } = null!;

    [Parameter] public int Gap { get; set; } = 4;
    [Parameter] public int TabHeight { get; set; } = 35;
    [Parameter] public List<TItem> Items { get; set; } = new();
    [Parameter] public Func<TItem, int>? TabWidthPredicate { get; set; } = x => 100;
    [Parameter] public Func<TItem, string>? DefaultTabText { get; set; }
    [Parameter] public int MinimalTabCount { get; set; }
    [Parameter] public int ScrollTabWidth { get; set; } = 100;
    [Parameter] public bool Draggable { get; set; } = false;
    [Parameter] public bool ScrollToSelected { get; set; } = true;
    [Parameter] public bool ShowCloseOnDefaultTab { get; set; } = true;
    
    [Parameter] public RenderFragment<TItem>? TabTemplate { get; set; } = null!;
    [Parameter] public RenderFragment<TItem>? ContentTemplate { get; set; } = null!;
    
    [Parameter] public EventCallback<TItem> SelectedChanged { get; set; }
    [Parameter] public TItem Selected
    {
        get => _selectedValue;
        set
        {
            if (_selectedValue == value) return;
            _selectedValue = value;

            SelectedChanged.InvokeAsync(value);
        }
    }

    private BCHModal? _modalComponent;
    private TItem _selectedValue = default!;
    private readonly string _contentId = $"_id_{Guid.NewGuid()}";

    private TabsDraggableContextModel<TItem> _draggableContext = new();
    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };
    private int _width;
    private int _height;
    private bool _isDragging = false;
    private bool _draggable = false;

    protected override void OnInitialized()
    {
        _draggable = Draggable;
        if (OwnerContainer == null) return;
        
        _draggableContext = OwnerContainer.DraggableContext;
        StateHasChanged();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        // Console.WriteLine("BCHTabs OnAfterRender");
    }

    private async Task OnItemDragAsync(Vec2 pos)
    {
        // Console.WriteLine($"x = {pos.X}, y = {pos.Y}");
        await _modalComponent!.SetPositionAsync(
            $"{(pos.X - _draggableContext.StartOffset.X).ToString(_nF)}px", 
            $"{(pos.Y - _draggableContext.StartOffset.Y).ToString(_nF)}px");
    }

    private async Task OnItemDragStartAsync()
    {
        var rectRect = await JsUtilsService.GetBoundingClientRectAsync(_contentId);
        // Console.WriteLine($"OnItemDragStartAsync {rectRect.Width} {rectRect.Height}");

        _width = (int) rectRect.Width;
        _height = (int) rectRect.Height;
        _isDragging = true;
        
        StateHasChanged();
    }
    
    private void OnItemDragEnd()
    {
        _isDragging = false;

        StateHasChanged();
        // Console.WriteLine("OnItemDragEnd");
    }

    private void OnCloseItem(TItem item)
    {
        Items.Remove(item);
        StateHasChanged();
    }
}