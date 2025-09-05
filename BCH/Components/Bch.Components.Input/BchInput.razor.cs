using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Bch.Modules.Themes.Models;
using Bch.Modules.Themes.Attributes;
using Bch.Modules.Themes.Extensions;

namespace Bch.Components.Input;

public partial class BchInput : ComponentBase
{
	[Parameter] public string CssClass { get; set; } = string.Empty;
	[Parameter] public string Placeholder { get; set; } = string.Empty;
	[Parameter] public string Type { get; set; } = "text"; // text, number, email, password
	[Parameter] public string? Value { get; set; }
	[Parameter] public EventCallback<string?> ValueChanged { get; set; }
	[Parameter] public EventCallback<string?> OnInput { get; set; }
	[Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }
	[Parameter] public EventCallback OnFocus { get; set; }
	[Parameter] public EventCallback OnBlur { get; set; }
	[Parameter] public bool Disabled { get; set; } = false;
	[Parameter] public bool ReadOnly { get; set; } = false;
	[Parameter] public bool ShowSearchIcon { get; set; } = false;
	[Parameter] public bool ShowClearButton { get; set; } = true;
	[Parameter] public int Width { get; set; } = 290;
	[Parameter] public int Height { get; set; } = 56;
	[Parameter] public int MaxLength { get; set; } = int.MaxValue;

	[CascadingParameter] public BchTheme? ThemeCascading { get; set; }
	[Parameter] public BchTheme? Theme { get; set; }

	private BchTheme EffectiveTheme => Theme ?? ThemeCascading ?? BchTheme.LightGreen;
	private string GetThemeCssClass()
	{
		var themeSpecified = Theme ?? ThemeCascading;
		return (EffectiveTheme.GetValue<string, CssNameAttribute>(a => a.CssName) ?? string.Empty) +
		       (themeSpecified is null ? " bch-no-theme-specified" : "");
	}

	private readonly string _containerId = $"_id_{Guid.NewGuid()}";
	private readonly string _inputId = $"_id_{Guid.NewGuid()}";
	private readonly string _cssKey = $"_cssKey_{Guid.NewGuid()}";
	private ElementReference _inputRef;
	private string _currentValue = string.Empty;
	private bool _showSearch => ShowSearchIcon && Type == "text";

	protected override void OnParametersSet()
	{
		_currentValue = Value ?? string.Empty;
	}

	private async Task OnInputChanged(ChangeEventArgs e)
	{
		_currentValue = e.Value?.ToString() ?? string.Empty;
		await ValueChanged.InvokeAsync(_currentValue);
		await OnInput.InvokeAsync(_currentValue);
		StateHasChanged();
	}

	private async Task OnKeyDownInternal(KeyboardEventArgs e)
	{
		await OnKeyDown.InvokeAsync(e);
	}

	private async Task ClearAsync()
	{
		_currentValue = string.Empty;
		await ValueChanged.InvokeAsync(_currentValue);
		await OnInput.InvokeAsync(_currentValue);
		StateHasChanged();
	}
}
