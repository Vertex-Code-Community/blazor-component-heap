using Microsoft.AspNetCore.Components;
using Bch.Components.RadioButton.RadioButtonsContainer;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.RadioButton.RadioButtonComponent;

public partial class BchRadioButton<TItem> : ComponentBase where TItem : class
{
    [CascadingParameter(Name = $"BCHRadioButtonContainer{nameof(TItem)}")] 
    public BchRadioButtonsContainer<TItem>? OwnerContainer { get; set; } = null!;

    [Parameter] public TItem Key { get; set; } = null!;

    [Parameter] public RenderFragment<bool>? ContentTemplate { get; set; }
    [Parameter] public RenderFragment<bool>? CircleTemplate { get; set; }
    [Parameter] public RenderFragment<bool>? DataTemplate { get; set; }

    [Parameter] public bool Disabled { get; set; }
    [Parameter] public BchTheme? Theme { get; set; }
    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }
    
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

    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
    private string GetThemeCssClass() => EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;
}

