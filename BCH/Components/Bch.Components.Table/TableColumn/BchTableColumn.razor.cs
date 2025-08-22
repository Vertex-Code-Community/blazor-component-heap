using Microsoft.AspNetCore.Components;
using Bch.Components.Table.Models;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.Table.TableColumn;

public partial class BchTableColumn<TRowData> : ComponentBase where TRowData : class
{
    [CascadingParameter] public required BchTable<TRowData> OwnerGrid { get; set; }
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string FilterProperty { get; set; } = string.Empty;
    [Parameter] public Func<TRowData, object> Expression { get; set; } = null!;
    [Parameter] public RenderFragment<TRowData>? TdTemplate { get; set; }
    [Parameter] public RenderFragment? ThTemplate { get; set; }
    [Parameter] public string Width { get; set; } = string.Empty;
    [Parameter] public ColumnFilterType FilterType { get; set; }
    [Parameter] public EventCallback<bool> OnClickSorted { get; set; }
    [Parameter] public bool IsSorted { get; set; }
    [Parameter] public BchTableColumn<TRowData> Column { get; set; } = null!;
    [Parameter] public EventCallback<TableFilterParameters> OnFilterData { get; set; }
    [Parameter] public string ThClass { get; set; } = string.Empty;
    [Parameter] public string TdClass { get; set; } = string.Empty;
    [Parameter] public List<string> SelectData { get; set; } = new();
    
    [Parameter] public BchTheme? Theme { get; set; }
    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }
    
    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
    private string GetThemeCssClass() => EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;
    
    protected override void OnInitialized()
    {
        OwnerGrid.AddColumn(this);
    }
}
