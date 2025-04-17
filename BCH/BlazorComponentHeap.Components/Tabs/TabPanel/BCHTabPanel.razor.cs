using System.Globalization;
using BlazorComponentHeap.Components.Models.Tab;
using BlazorComponentHeap.Core.Models.Events;
using BlazorComponentHeap.Core.Models.Markup;
using BlazorComponentHeap.Core.Models.Math;
using BlazorComponentHeap.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.Components.Tabs.TabPanel;

public partial class BCHTabPanel<TItem> : ComponentBase, IAsyncDisposable where TItem : class
{
    [Inject] private IJSUtilsService JsUtilsService { get; set; } = null!;

    [Parameter] public int Gap { get; set; }
    [Parameter] public string CssClass { get; set; } = string.Empty;
    [Parameter] public List<TItem> Items { get; set; } = new();
    [Parameter] public RenderFragment<TItem> ItemTemplate { get; set; } = null!;
    [Parameter] public TabsDraggableContextModel<TItem> DraggableContext { get; set; } = null!;
    [Parameter] public Func<TItem, int> TabWidthPredicate { get; set; } = x => 100;
    [Parameter] public Func<Vec2, Task>? OnItemDrag { get; set; }
    [Parameter] public Func<Task>? OnItemDragStart { get; set; }
    [Parameter] public Action? OnItemDragEnd { get; set; }
    [Parameter] public int MinimalTabCount { get; set; }
    [Parameter] public bool Draggable { get; set; }

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

    private TItem _selectedValue = default!;
    
    private TItem? _draggingItem = null!; // determines if item is dragging
    private TItem? _hoveredItem = null!; // determines if item is dragging
    
    private bool _direction = true;
    private bool _startDrag = true;

    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };
    private readonly string _subscriptionKey = $"_id_{Guid.NewGuid()}";
    private readonly string _panelId = $"_id_{Guid.NewGuid()}";

    private TItem? _pressedItem = null;
    private int _minimalTabCount = 1;
    private int _prevItemsCount = 0;

    protected override void OnInitialized()
    {
        if (MinimalTabCount > _minimalTabCount) _minimalTabCount = MinimalTabCount;
        Selected = Items[0];
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            DraggableContext.OnUpdate += StateHasChanged;
            DraggableContext.OnMove += OnDocumentMove;

            await JsUtilsService.AddDocumentListenerAsync<ExtMouseEventArgs>("mouseup", _subscriptionKey,
                OnDocumentUpAsync);
            await JsUtilsService.AddDocumentListenerAsync<ExtTouchEventArgs>("touchend", _subscriptionKey,
                OnDocumentTouchEndAsync);
        }
        
        // Console.WriteLine("BCHNTabPanel OnAfterRender");
    }

    public async ValueTask DisposeAsync()
    {
        DraggableContext.OnUpdate -= StateHasChanged; // TODO: unsubscribe if subscribed only
        DraggableContext.OnMove -= OnDocumentMove; // TODO: unsubscribe if subscribed only
        
        await JsUtilsService.RemoveDocumentListenerAsync<ExtMouseEventArgs>("mousemove", _subscriptionKey);
        await JsUtilsService.RemoveDocumentListenerAsync<ExtTouchEventArgs>("touchmove", _subscriptionKey);
        await JsUtilsService.RemoveDocumentListenerAsync<ExtMouseEventArgs>("mouseup", _subscriptionKey);
        await JsUtilsService.RemoveDocumentListenerAsync<ExtTouchEventArgs>("touchend", _subscriptionKey);
    }

    protected override void OnParametersSet()
    {
        if (_prevItemsCount == Items.Count) return;
        _prevItemsCount = Items.Count;
        
        if (_prevItemsCount == 0) return;

        if (!Items.Contains(Selected))
        {
            Selected = Items[0];
            StateHasChanged();
        }
        
        // Console.WriteLine("Tabs count changed");
    }

    internal void OnUpdateItemsCount()
    {
        OnParametersSet();
    }

    private async Task OnMouseDownAsync(TItem item, float pageX, float pageY, float x, float y, List<CoordsHolder> pathCoordinates)
    {
        var itemHolder = pathCoordinates.FirstOrDefault(x => x.ClassList.Contains($"_cls_item_{_panelId}"));
        var removeHolder = pathCoordinates.FirstOrDefault(x => x.ClassList.Contains("bch-tab-control-btn"));
        if (itemHolder == null! || removeHolder != null!) return;

        if (Draggable)
        {
            await JsUtilsService.AddDocumentListenerAsync<ExtMouseEventArgs>("mousemove", _subscriptionKey,
                OnDocumentMouseMove);
            await JsUtilsService.AddDocumentListenerAsync<ExtTouchEventArgs>("touchmove", _subscriptionKey,
                OnDocumentTouchMove);
        }
        
        DraggableContext.StartOffset.Set(itemHolder.X, itemHolder.Y);
        DraggableContext.DraggingPosition.Set(pageX, pageY);
        DraggableContext.DraggingBounds.Set(itemHolder.ClientWidth, itemHolder.ClientHeight);
        _pressedItem = item;
    }
    
    private Task OnDocumentTouchMove(ExtTouchEventArgs e)
    {
        if (e.Touches.Count != 1) return Task.CompletedTask;
        var touch = e.Touches.First();
        
        DraggableContext.OnMove?.Invoke(touch.PageX, touch.PageY, touch.PathCoordinates);

        return Task.CompletedTask;
    }

    private Task OnDocumentMouseMove(ExtMouseEventArgs e)
    {
        DraggableContext.OnMove?.Invoke((float)e.PageX, (float)e.PageY, e.PathCoordinates);
        
        return Task.CompletedTask;
    }
    
    private void OnDocumentMove(float pageX, float pageY, List<CoordsHolder> pathCoordinates)
    {
        if (Items.Count <= _minimalTabCount && _pressedItem != null) return;

        if (_pressedItem is not null)
        {
            var startPos = DraggableContext.DraggingPosition;
            var pos = new Vec2(startPos.X - pageX, startPos.Y - pageY);

            if (!(pos.Length() > 3)) return;
            
            _draggingItem = _pressedItem;
            DraggableContext.DraggingItem = _pressedItem;
            DraggableContext.Items = Items;
            DraggableContext.TabPanelInstance = this;
            _startDrag = true;
            
            var draggingIndex = Items.IndexOf(_pressedItem);
            if (draggingIndex < Items.Count - 1) _hoveredItem = Items[draggingIndex + 1];
            
            _pressedItem = null;
            
            StateHasChanged();
            OnItemDragStart?.Invoke();
            DraggableContext.OnRerenderScroller?.Invoke();

            return;
        }
        
        var panelHolder = pathCoordinates.FirstOrDefault(x => x.Id == _panelId);
        if (_draggingItem == null! && (DraggableContext.DraggingItem == null! || panelHolder == null!)) return;

        _startDrag = false;

        var prevHovered = _hoveredItem;
        var prevDirection = _direction;
        
        var (direction, index) = GetItemHolder(pathCoordinates);

        if (index != -1)
        {
            _hoveredItem = Items[index];
            _direction = direction;
        }
        else
        {
            _hoveredItem = null;
        }

        if (_draggingItem != null!)
        {
            DraggableContext.DraggingPosition.Set(pageX, pageY);
            OnItemDrag?.Invoke(DraggableContext.DraggingPosition);
        }

        if (prevHovered != _hoveredItem || prevDirection != _direction) StateHasChanged();
    }

    private Task OnDocumentTouchEndAsync(ExtTouchEventArgs e)
    {
        if (e.Touches.Count != 1) return Task.CompletedTask;
        var touch = e.Touches.First();
        
        return OnDocumentUpAsync(new ExtMouseEventArgs
        {
            PageX = touch.PageX,
            PageY = touch.PageY,
            PathCoordinates = touch.PathCoordinates
        });
    }

    private async Task OnDocumentUpAsync(ExtMouseEventArgs e)
    {
        var (direction, index) = GetItemHolder(e.PathCoordinates);
        var ind = index;
        var panelHolder = e.PathCoordinates.FirstOrDefault(x => x.Id == _panelId);

        if (panelHolder != null && _draggingItem != null) // place into this
        {
            // Console.WriteLine("place into this");
            var draggingIndex = Items.IndexOf(_draggingItem);
            if (index > draggingIndex) index--;
            if (!direction) index++;

            Items.Remove(_draggingItem);

            if (ind == -1) index = Items.Count;
            Items.Insert(index, _draggingItem);
            
            DraggableContext.DraggingItem = null;
            DraggableContext.Items = null;
            DraggableContext.TabPanelInstance = null;
        }

        if (panelHolder != null && _draggingItem == null && DraggableContext.DraggingItem != null) // drop from other
        {
            // Console.WriteLine("drop from other");
            DraggableContext.Items?.Remove(DraggableContext.DraggingItem);
            DraggableContext.OnUpdate?.Invoke();
            DraggableContext.TabPanelInstance?.OnUpdateItemsCount();

            if (!direction) index++;
            if (ind == -1) index = Items.Count;
            Items.Insert(index, DraggableContext.DraggingItem);

            DraggableContext.DraggingItem = null;
            DraggableContext.Items = null;
            DraggableContext.TabPanelInstance = null;
        }

        if (panelHolder == null && _draggingItem == _selectedValue) // drop selected outside of panel
        {
            // Console.WriteLine("drop selected outside of panel");
            var otherPanelIdentifier = e.PathCoordinates
                .FirstOrDefault(x => x.ClassList.Contains(DraggableContext.ContextIdentifier));

            if (otherPanelIdentifier is null) // 100% dropped selected outside of panel
            {
                DraggableContext.DraggingItem = null;
                DraggableContext.Items = null;
                DraggableContext.TabPanelInstance = null;
            }
        }
        
        var item = _pressedItem;

        _draggingItem = null;
        // DraggableContext.DraggingItem = null;
        // DraggableContext.Items = null;
        _hoveredItem = null;
        _direction = true;
        _startDrag = true;
        _pressedItem = null;

        if (item != null)
        {
            var startPos = DraggableContext.DraggingPosition;
            var pos = new Vec2(startPos.X - e.PageX, startPos.Y - e.PageY);

            if (pos.Length() <= 3)
            {
                OnItemClicked(item);
            }
        }
        else
        {
            OnItemDragEnd?.Invoke();
        }
        
        StateHasChanged();
        
        DraggableContext.OnUpdateScroller?.Invoke();

        if (Draggable)
        {
            await JsUtilsService.RemoveDocumentListenerAsync<ExtMouseEventArgs>("mousemove", _subscriptionKey);
            await JsUtilsService.RemoveDocumentListenerAsync<ExtTouchEventArgs>("touchmove", _subscriptionKey);
        }
    }

    // needed for mobile, when long-click causes context menu opening and we need to cancel on mousedown changes in state
    private Task OnContextMenuAsync()
    {
        return OnDocumentUpAsync(new ExtMouseEventArgs());
    }

    private void OnItemClicked(TItem item)
    {
        Selected = item;
    }

    private (bool, int) GetItemHolder(List<CoordsHolder> pathCoordinates)
    {
        var itemHolder = pathCoordinates.FirstOrDefault(x => x.ClassList.Contains($"_cls_item_{_panelId}"));
        if (itemHolder is null) return (false, -1);
        var classes = itemHolder.ClassList.Split(" ");
        if (classes[0] != "bch-item") return (false, -1);
        
        var indexStr = classes[1].Replace("n-", "");
        var index = int.Parse(indexStr);
        var direction = itemHolder.X < itemHolder.ClientWidth * 0.5f;
            
        return (direction, index);
    }
}