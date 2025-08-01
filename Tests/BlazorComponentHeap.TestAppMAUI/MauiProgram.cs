using Microsoft.Extensions.Logging;
using BlazorComponentHeap.DomInterop.Extensions;
using BlazorComponentHeap.GlobalEvents.Extensions;
using BlazorComponentHeap.Modal.Extensions;
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