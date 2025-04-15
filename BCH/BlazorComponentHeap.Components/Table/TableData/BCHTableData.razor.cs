using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Components.Table.TableData;

public partial class BCHTableData<TRowData> : ComponentBase where TRowData : class
{
    [Parameter] public Func<TRowData, object> Expression { get; set; } = null!;
    [Parameter] public TRowData Item { get; set; } = null!;
    [Parameter] public string Width { get; set; } = string.Empty;

    // protected override void OnInitialized()
    // {
    //     var res = Expression.Invoke(Item).ToString();
    // }
}
