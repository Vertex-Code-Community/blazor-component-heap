using Microsoft.AspNetCore.Components;
using Bch.Components.Table.Models;
using Bch.Components.Table.TableColumn;
using Bch.Modules.GlobalEvents.Events;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.Table;

public partial class BchTable<TRowData> : ComponentBase
    where TRowData : class
{
    [Parameter] public ICollection<TRowData> Items { get; set; } = new List<TRowData>();
    [Parameter] public required RenderFragment ChildContent { get; set; }
    [Parameter] public EventCallback<TableFilterParameters> OnFilterData { get; set; }
    [Parameter] public EventCallback<TableSortParameters> OnSortData { get; set; }
    [Parameter] public string MinWidth { get; set; } = "670px";
    [Parameter] public BchTheme? Theme { get; set; }
    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }

    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
    private readonly string _cssKey = $"_cssKey_{Guid.NewGuid()}";
    private string GetThemeCssClass()
    {
        var themeSpecified = Theme ?? ThemeCascading;
        var themeClass = EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;
        return themeClass + (themeSpecified is null ? " bch-no-theme-specified" : "");
    }

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

    private readonly List<BchTableColumn<TRowData>> columns = new List<BchTableColumn<TRowData>>();
    private readonly string _containerId = $"_id_{Guid.NewGuid()}";
    
    internal void AddColumn(BchTableColumn<TRowData> column)
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

    private void OnScroll(BchScrollEventArgs e)
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
