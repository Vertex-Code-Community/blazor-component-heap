using Microsoft.Extensions.Logging;
using Bch.Modules.DomInterop.Extensions;
using Bch.Modules.Files.Extensions;
using Bch.Modules.GlobalEvents.Extensions;
using Bch.Components.Modal.Extensions;

namespace Bch.Integration.Maui;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        var services = builder.Services;

        services.AddBchModal();
        services.AddBchFiles();
        services.AddBchDomInterop();
        services.AddBchGlobalEvents();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
