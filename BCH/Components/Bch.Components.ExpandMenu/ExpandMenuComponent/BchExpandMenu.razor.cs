using Microsoft.AspNetCore.Components;
using Bch.Components.ExpandMenu.ExpandMenuContainer;
using Bch.Modules.DomInterop.Services;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.ExpandMenu.ExpandMenuComponent;

public partial class BchExpandMenu : IDisposable
{
    [Inject] public required IDomInteropService DomInteropService { get; set; }

    [CascadingParameter(Name = "BCHExpandMenuContainer")] public BchExpandMenuContainer OwnerContainer { get; set; } = null!;
    
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string Description { get; set; } = string.Empty;
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public BchTheme? Theme { get; set; }
    [CascadingParameter] public BchTheme? ThemeCascading { get; set; }

    private readonly string _descriptionId = $"_id_{Guid.NewGuid()}";
    private double _heightDescription = 0;

    private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
    private string GetThemeCssClass() => EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty;

    protected override void OnInitialized()
    {
        OwnerContainer.AddExpandMenu(this);
        // IJSUtilsService.OnResize += OnResizeAsync;
    }
    
    public void Dispose()
    {
        // IJSUtilsService.OnResize -= OnResizeAsync;
    }

    private async Task OnResizeAsync()
    {
        var boundingClientRect = await DomInteropService.GetBoundingClientRectAsync(_descriptionId);
        if (boundingClientRect is null) return;
        
        _heightDescription = boundingClientRect.OffsetHeight;
        StateHasChanged();
    }

    private async Task OnClick()
    {
        await OnResizeAsync();
        if (Disabled) return;

        OwnerContainer.SelectButton(this);
    }

    public void Update() => StateHasChanged();
}
