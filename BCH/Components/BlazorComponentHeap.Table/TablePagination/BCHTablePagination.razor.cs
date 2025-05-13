using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Table.TablePagination;

public partial class BCHTablePagination : ComponentBase
{
    [Parameter] public int CurrentPage { get; set; }
    [Parameter] public int PageSize
    {
        get => _pageSize;
        set
        {
            if (_pageSize == value) return;
            _pageSize = value;
            PageSizeChanged.InvokeAsync(value);
            PageSizeSelectChanged.InvokeAsync(value);
        }
    }

    [Parameter] public EventCallback<int> PageSizeChanged { get; set; }
    [Parameter] public EventCallback<int> PageSizeSelectChanged { get; set; }
    [Parameter] public int TotalItems { get; set; }
    [Parameter] public EventCallback<int> OnPageNumber { get; set; }
    [Parameter] public List<uint> Sizes { get; set; } = new() { 5, 10, 20 };

    private int _pageSize = 5;
    private int _prevCurrentPage = -1;
    private int _totalPages => (int)Math.Ceiling((decimal)TotalItems / PageSize);
    private readonly string _containerId = $"_id_{Guid.NewGuid()}";
    
    private async Task OnPaginationAsync(bool isNextNumber)
    {
        if (isNextNumber && CurrentPage >= _totalPages)
        {
            return;
        }

        CurrentPage = Math.Clamp(CurrentPage + (Convert.ToInt32(isNextNumber) * 2 - 1), 1, TotalItems);

        if (CurrentPage != _prevCurrentPage)
        {
            _prevCurrentPage = CurrentPage;
            await OnPageNumber.InvokeAsync(CurrentPage);
        }
    }

    private async Task OnSelectPageNumberAsync(object pageNumber)
    {
        if (pageNumber.GetType() == typeof(int))
        {
            CurrentPage = (int)pageNumber;
            await OnPageNumber.InvokeAsync(CurrentPage);
        }
    }

    private string GetActivePage(object pageNumber)
    {
        if (pageNumber.GetType() == typeof(int))
        {
            return CurrentPage == (int)pageNumber ? "active" : string.Empty;
        }

        return string.Empty;
    }

    private static List<object> GetPages(int current, int pageCount)
    {
        var pages = new List<object>();
        var delta = 7;

        if (pageCount > 7)
        {
            delta = current > 4 && current < pageCount - 3 ? 2 : 4;
        }

        var startIndex = (int)Math.Round(current - delta / (double)2);
        var endIndex = (int)Math.Round(current + delta / (double)2);

        if (startIndex - 1 == 1 || endIndex + 1 == pageCount)
        {
            startIndex += 1;
            endIndex += 1;
        }

        var to = Math.Min(pageCount, delta + 1);
        for (int i = 1; i <= to; i++)
        {
            pages.Add(i);
        }

        if (current > delta)
        {
            pages.Clear();
            var from = Math.Min(startIndex, pageCount - delta);
            to = Math.Min(endIndex, pageCount);
            for (int i = from; i <= to; i++)
            {
                pages.Add(i);
            }
        }

        if (pages.Count == 0) return pages;

        if (pages[0].ToString() != "1")
        {
            if (pages.Count() + 1 != pageCount)
            {
                pages.Insert(0, "...");
            }
            pages.Insert(0, 1);
        }

        if ((int)pages.Last() < pageCount)
        {
            if (pages.Count() + 1 != pageCount)
            {
                pages.Add("...");
            }
            pages.Add(pageCount);
        }

        return pages;
    }
}