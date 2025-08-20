using Microsoft.AspNetCore.Components;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.Table.TableData;

public partial class BchTableData<TRowData> : ComponentBase where TRowData : class
{
    [Parameter] public Func<TRowData, object> Expression { get; set; } = null!;
    [Parameter] public TRowData Item { get; set; } = null!;
    [Parameter] public string Width { get; set; } = string.Empty;

    [Parameter] public BchTheme? Theme { get; set; }
    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }
    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
    private string GetThemeCssClass() => EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;

    // protected override void OnInitialized()
    // {
    //     var res = Expression.Invoke(Item).ToString();
    // }
}
