using Bch.Components.Calendar.Models;
using Bch.Components.Table.Models;
using Bch.Components.Table.TableColumn;
using Microsoft.AspNetCore.Components;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.Table.TableHead;

public partial class BchTableHead<TRowData> : ComponentBase where TRowData : class
{
    [Parameter] public string Width { get; set; } = string.Empty;
    [Parameter] public BchTableColumn<TRowData> Column { get; set; } = null!;
    [Parameter] public EventCallback<TableFilterParameters> OnFilterData { get; set; }
    [Parameter] public EventCallback<TableSortParameters> OnClickSorted { get; set; }
    [Parameter] public BchTheme? Theme { get; set; }
    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }

    private readonly string _containerId = $"_id_{Guid.NewGuid()}";
    private readonly string _calendarTableClass = $"_class_{Guid.NewGuid()}";
    private readonly string _selectTableClass = $"_class_{Guid.NewGuid()}";
    private List<string> _selectedItems = new();
    private List<DateTime> _selectedDates = new();
    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
    private string GetThemeCssClass() => EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;

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

    private Task OnSelectDateAsync(DateTime? date, string columnName)
    {
        return OnFilterData.InvokeAsync(new TableFilterParameters
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