using BlazorComponentHeap.RadioButton.RadioButton.RadioButton;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.RadioButton.RadioButton.RadioButtonsContainer;

public partial class BCHRadioButtonsContainer<TItem> : ComponentBase where TItem : class
{
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public EventCallback<TItem> SelectedChanged { get; set; }
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
    private readonly List<BCHRadioButton<TItem>> _radioButtons = new();
    private bool _firstRender = false;

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

    internal BCHRadioButton<TItem>? SelectedRb { get; set; } = null!;

    internal void AddRadioButton(BCHRadioButton<TItem> radioButton)
    {
        var rb = _radioButtons.FirstOrDefault(x => x.Key == radioButton.Key);
        if (rb != null) throw new Exception($"BCHRadioButton text duplication, {radioButton.Key}.");
        
        _radioButtons.Add(radioButton);
    }

    internal void SelectButton(BCHRadioButton<TItem> radioButton)
    {
        SelectRb(radioButton);

        foreach (var rb in _radioButtons)
        {
            rb.Update();
        }
    }

    private void SelectRb(BCHRadioButton<TItem> radioButton)
    {
        SelectedRb = radioButton;
        _selectedValue = radioButton.Key;
        SelectedChanged.InvokeAsync(_selectedValue);
    }
}

