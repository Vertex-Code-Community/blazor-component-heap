namespace Bch.Integration.Pages.Pages.Themes.ThemeControl;

public partial class ThemeControlComponent
{
    private readonly List<Bch.Modules.Themes.Models.BchTheme> _themes = new()
    {
        Bch.Modules.Themes.Models.BchTheme.LightGreen,
        Bch.Modules.Themes.Models.BchTheme.Light,
        Bch.Modules.Themes.Models.BchTheme.Dark
    };

    private Bch.Modules.Themes.Models.BchTheme _selectedTheme = Bch.Modules.Themes.Models.BchTheme.Dark;

    private string GetThemeName(Bch.Modules.Themes.Models.BchTheme theme)
    {
        return theme.ToString();
    }

    private Task OnThemeChangedAsync(Bch.Modules.Themes.Models.BchTheme? theme)
    {
        if (theme.HasValue)
        {
            _selectedTheme = theme.Value;
        }
        return Task.CompletedTask;
    }
}