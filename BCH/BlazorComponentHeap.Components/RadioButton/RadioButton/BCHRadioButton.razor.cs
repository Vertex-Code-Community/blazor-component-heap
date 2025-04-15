using System;
using Microsoft.AspNetCore.Components;
using System.Xml.Linq;
using BlazorComponentHeap.Components.RadioButton.RadioButtonsContainer;

namespace BlazorComponentHeap.Components.RadioButton.RadioButton;

public partial class BCHRadioButton<TItem> : ComponentBase where TItem : class
{
    [CascadingParameter(Name = $"BCHRadioButtonContainer{nameof(TItem)}")] 
    public BCHRadioButtonsContainer<TItem>? OwnerContainer { get; set; } = null!;

    [Parameter] public TItem Key { get; set; } = null!;

    [Parameter] public RenderFragment<bool>? ContentTemplate { get; set; } = null!;
    [Parameter] public RenderFragment<bool>? CircleTemplate { get; set; } = null!;
    [Parameter] public RenderFragment<bool>? DataTemplate { get; set; } = null!;

    [Parameter] public bool Disabled { get; set; }
    
    protected override void OnInitialized()
    {
        if (OwnerContainer is null) throw new Exception("Looks like radio buttons should be wrapped with container");
        
        OwnerContainer.AddRadioButton(this);
    }

    private void OnClick()
    {
        OwnerContainer?.SelectButton(this);
    }

    public void Update() => StateHasChanged();
}

