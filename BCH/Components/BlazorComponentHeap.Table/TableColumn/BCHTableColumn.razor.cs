using BlazorComponentHeap.Table.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Table.TableColumn;

public partial class BCHTableColumn<TRowData> : ComponentBase where TRowData : class
{
    [CascadingParameter] public BCHTable<TRowData> OwnerGrid { get; set; } = null!;
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string FilterProperty { get; set; } = string.Empty;
    [Parameter] public Func<TRowData, object> Expression { get; set; } = null!;
    [Parameter] public RenderFragment<TRowData>? TdTemplate { get; set; } = null!;
    [Parameter] public RenderFragment? ThTemplate { get; set; } = null!;
    [Parameter] public string Width { get; set; } = string.Empty;
    [Parameter] public ColumnFilterType FilterType { get; set; }
    [Parameter] public EventCallback<bool> OnClickSorted { get; set; }
    [Parameter] public bool IsSorted { get; set; }
    [Parameter] public BCHTableColumn<TRowData> Column { get; set; } = null!;
    [Parameter] public EventCallback<TableFilterParameters> OnFilterData { get; set; }
    [Parameter] public string ThClass { get; set; } = string.Empty;
    [Parameter] public string TdClass { get; set; } = string.Empty;
    [Parameter] public List<string> SelectData { get; set; } = new();
    
    protected override void OnInitialized()
    {
        OwnerGrid.AddColumn(this);
    }
}
