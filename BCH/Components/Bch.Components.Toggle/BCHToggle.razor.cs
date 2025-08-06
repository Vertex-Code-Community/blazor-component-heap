using Microsoft.AspNetCore.Components;

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

    private string _toggleId = $"_id_{Guid.NewGuid()}";
}
