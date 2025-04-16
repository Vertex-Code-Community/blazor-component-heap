using BlazorComponentHeap.Core.Models.Datepicker;
using BlazorComponentHeap.Core.Models.Table;
using BlazorComponentHeap.Table.Table.TableColumn;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Table.Table.TableHead;

public partial class BCHTableHead<TRowData> : ComponentBase where TRowData : class
{
    [Parameter] public string Width { get; set; } = string.Empty;
    [Parameter] public BCHTableColumn<TRowData> Column { get; set; } = null!;
    [Parameter] public EventCallback<TableFilterParameters> OnFilterData { get; set; }
    [Parameter] public EventCallback<TableSortParameters> OnClickSorted { get; set; }

    private readonly string _containerId = $"_id_{Guid.NewGuid()}";
    private readonly string _calendarTableClass = $"_class_{Guid.NewGuid()}";
    private readonly string _selectTableClass = $"_class_{Guid.NewGuid()}";
    private List<string> _selectedItems = new();
    private List<DateTime> _selectedDates = new();

    private async Task OnFilterAsync(ChangeEventArgs changeEvent, string columnName, bool isMultiple = false)
    {
        var listData = new List<string>();

        if (isMultiple)
        {
            var list = changeEvent.Value as string[];
            if (list == null) return;

            list.ToList().ForEach(item => listData.Add(item.ToString()));
        }
        else
        {
            listData.Add(changeEvent!.Value!.ToString()!);
        }

        var selectedData = new TableFilterParameters
        {
            PropertyName = columnName,
            Filters = listData
        };

        await OnFilterData.InvokeAsync(selectedData);
    }

    private async Task OnSelectedAsync(string columnName)
    {
        await OnFilterData.InvokeAsync(new TableFilterParameters
        {
            PropertyName = columnName,
            Filters = _selectedItems.ToList()
        });
    }

    private async Task OnSelectedAsync(string selected, string columnName)
    {
        await OnFilterData.InvokeAsync(new TableFilterParameters
        {
            PropertyName = columnName,
            Filters = new List<string> { selected }
        });
    }

    private async Task OnSortedAsync(string columnName)
    {
        if (Column.OwnerGrid.PrevSortPropertyName != columnName)
        {
            Column.OwnerGrid.PrevSortPropertyName = columnName;
            Column.OwnerGrid.Sorted = true;
        }
        else
        {
            Column.OwnerGrid.Sorted = !Column.OwnerGrid.Sorted;
        }

        await OnClickSorted.InvokeAsync(new TableSortParameters
        {
            PropertyName = columnName,
            OrderByAsc = Column.OwnerGrid.Sorted
        });
    }

    private async Task OnSelectDateAsync(DateTime date, string columnName)
    {
        await OnFilterData.InvokeAsync(new TableFilterParameters
        {
            PropertyName = columnName,
            Date = date
        });
    }

    private async Task OnSelectDateAsync(DateRange date, string columnName)
    {
        await OnFilterData.InvokeAsync(new TableFilterParameters
        {
            PropertyName = columnName,
            DateRange = date
        });
    }
}