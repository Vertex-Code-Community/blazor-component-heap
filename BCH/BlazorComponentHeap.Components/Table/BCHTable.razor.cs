using BlazorComponentHeap.Components.Table.TableColumn;
using BlazorComponentHeap.Core.Services.Interfaces;
using BlazorComponentHeap.Shared.Models.Events;
using BlazorComponentHeap.Shared.Models.Table;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Components.Table;

public partial class BCHTable<TRowData> : ComponentBase
    where TRowData : class
{
    [Parameter] public ICollection<TRowData> Items { get; set; } = new List<TRowData>();
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public EventCallback<TableFilterParameters> OnFilterData { get; set; }
    [Parameter] public EventCallback<TableSortParameters> OnSortData { get; set; }
    [Parameter] public string MinWidth { get; set; } = "670px";

    #region Pagination
    [Parameter] public bool IsPagination { get; set; }
    [Parameter] public bool IsScrollPagination { get; set; }
    [Parameter] public int PageSize { get; set; }
    [Parameter] public List<uint> Sizes { get; set; } = new() { 5, 10, 20 };

    [Parameter] public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage == value) return;
            _currentPage = value;
            CurrentPageChanged.InvokeAsync(value);
        }
    }
    
    [Parameter] public EventCallback<int> CurrentPageChanged { get; set; }

    private int _currentPage = 1;
    private bool _prevInBottom = false;
    public int _totalItems => Items.Count();
    private int _previousPageSize;
    private int _pageSize  = 5;

    internal bool Sorted = true;
    internal string PrevSortPropertyName = string.Empty;

    #endregion

    private List<TRowData> _items { get; set; } = new List<TRowData>();

    private readonly List<BCHTableColumn<TRowData>> columns = new List<BCHTableColumn<TRowData>>();
    private readonly string _containerId = $"_id_{Guid.NewGuid()}";
    
    internal void AddColumn(BCHTableColumn<TRowData> column)
    {
        columns.Add(column);
    }

    protected override void OnInitialized()
    {
        CurrentPage = 1;
        _items.Clear();
        _pageSize = PageSize == 0 ? Items.Count() : PageSize;
        _previousPageSize = _pageSize;
        ReloadItems();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            StateHasChanged();
        }

        if (_previousPageSize != _pageSize)
        {
            _previousPageSize = _pageSize;
            ReloadItems();
        }
    }

    private void OnPagination(int pageNumber)
    {
        CurrentPage = pageNumber;
        ReloadItems();
    }

    private void OnScroll(ScrollEventArgs e)
    {
        if (IsPagination && IsScrollPagination)
        {
            var inBottom = (e.ScrollTop + e.ClientHeight) >= e.ScrollHeight - 1;

            if (inBottom && !_prevInBottom)
            {
                CurrentPage++;
                ReloadItems();
            }

            _prevInBottom = inBottom;
        }
    }

    private void ReloadItems()
    {
        if (IsPagination && IsScrollPagination)
        {
            _items.AddRange(Items.Skip((CurrentPage - 1) * _pageSize).Take(_pageSize).ToList());
            StateHasChanged();
            return;
        }

        _items = Items.Skip((CurrentPage - 1) * _pageSize).Take(_pageSize).ToList();
        StateHasChanged();
    }

    public void ReloadTable()
    {
        OnInitialized();
    }
}
