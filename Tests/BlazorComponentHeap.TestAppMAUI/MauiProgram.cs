using Bch.Components.Modal.Extensions;
using Bch.Modules.DomInterop.Extensions;
using Bch.Modules.GlobalEvents.Extensions;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace BlazorComponentHeap.TestAppMAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        var services = builder.Services;
        services.AddMauiBlazorWebView();

#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        services.AddBchModal();
        services.AddBchDomInterop();
        services.AddBchGlobalEvents();

        return builder.Build();
    }
}