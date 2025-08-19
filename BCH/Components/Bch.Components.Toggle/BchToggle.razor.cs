using Microsoft.AspNetCore.Components;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.Toggle;

public partial class BchToggle
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public EventCallback<bool> ValueChanged { get; set; }
    [Parameter] public bool Value
    {
        get => _valueChanged;
        set
        {
            if (_valueChanged == value) return;
            _valueChanged = value;

            ValueChanged.InvokeAsync(value);
        }
    }

    private bool _valueChanged;

    [Parameter] public string CheckedCircleColor { get; set; } = string.Empty;
    [Parameter] public string CheckedBackgroundColor { get; set; } = string.Empty;
    [Parameter] public string DefaultCircleColor { get; set; } = string.Empty;
    [Parameter] public string DefaultBackgroundColor { get; set; } = string.Empty;
    [Parameter] public BchTheme? Theme { get; set; }
    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }

    private string _toggleId = $"_id_{Guid.NewGuid()}";
    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
    private string GetThemeCssClass() => EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;

    private string ResolveDefaultBackgroundColor()
        => string.IsNullOrWhiteSpace(DefaultBackgroundColor)
            ? "#D7D7D7"
            : DefaultBackgroundColor;

    private string ResolveDefaultCircleColor()
        => string.IsNullOrWhiteSpace(DefaultCircleColor)
            ? "#FFFFFF"
            : DefaultCircleColor;

    private string ResolveCheckedCircleColor()
        => string.IsNullOrWhiteSpace(CheckedCircleColor)
            ? "#FFFFFF"
            : CheckedCircleColor;

    private string ResolveCheckedBackgroundColor()
        => string.IsNullOrWhiteSpace(CheckedBackgroundColor)
            ? EffectiveTheme switch
            {
                BchTheme.Dark => "#4B5563",
                BchTheme.Light => "#6C757D",
                _ => "#33B469"
            }
            : CheckedBackgroundColor;
}
