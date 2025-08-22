using Bch.Components.RadioButton.RadioButtonComponent;
using Microsoft.AspNetCore.Components;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.RadioButton.RadioButtonsContainer;

public partial class BchRadioButtonsContainer<TItem> : ComponentBase where TItem : class
{
    [Parameter] public required RenderFragment ChildContent { get; set; }
    [Parameter] public EventCallback<TItem> SelectedChanged { get; set; }
    [Parameter] public BchTheme? Theme { get; set; }
    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }
    [Parameter] public TItem Selected
    {
        get => _selectedValue;
        set
        {
            if (_selectedValue == value) return;

            if (_firstRender)
            {
                var rb = _radioButtons.FirstOrDefault(x => x.Key == value);
                if (rb == null) return;
                
                SelectButton(rb);
            }
            
            _selectedValue = value;
            SelectedChanged.InvokeAsync(value);
        }
    }

    private TItem _selectedValue = null!;
    private readonly List<BchRadioButton<TItem>> _radioButtons = new();
    private bool _firstRender = false;
    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
    private string GetThemeCssClass() => EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;

    protected override void OnAfterRender(bool firstRender)
    {
        _firstRender = firstRender;
        if (!firstRender) return;
        
        var rb = _radioButtons.FirstOrDefault(x => x.Key == Selected);
        if (rb == null)
        {
            if (_radioButtons.Count == 0)
            {
                _selectedValue = null!;
                SelectedChanged.InvokeAsync(_selectedValue);
            }
            else
            {
                Selected = _radioButtons[0].Key;
            }
            
            return;
        }
        
        SelectButton(rb);
    }

    internal BchRadioButton<TItem>? SelectedRb { get; set; } = null!;

    internal void AddRadioButton(BchRadioButton<TItem> radioButton)
    {
        var rb = _radioButtons.FirstOrDefault(x => x.Key == radioButton.Key);
        if (rb != null) throw new Exception($"BCHRadioButton text duplication, {radioButton.Key}.");
        
        _radioButtons.Add(radioButton);
    }

    internal void SelectButton(BchRadioButton<TItem> radioButton)
    {
        SelectRb(radioButton);

        foreach (var rb in _radioButtons)
        {
            rb.Update();
        }
    }

    private void SelectRb(BchRadioButton<TItem> radioButton)
    {
        SelectedRb = radioButton;
        _selectedValue = radioButton.Key;
        SelectedChanged.InvokeAsync(_selectedValue);
    }
}

