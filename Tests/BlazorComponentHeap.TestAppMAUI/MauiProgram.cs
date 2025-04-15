using BlazorComponentHeap.Core.Extensions;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

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

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.AddBCHComponents("_live_key_af3ca774-12d8-4622-b4b5-b1c1b6bf3621");

        return builder.Build();
    }
}