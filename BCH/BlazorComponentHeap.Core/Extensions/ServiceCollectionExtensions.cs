using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using BlazorComponentHeap.Core.Options;
using BlazorComponentHeap.Core.Providers;
using BlazorComponentHeap.Core.Services;
using BlazorComponentHeap.Core.Services.Interfaces;

namespace BlazorComponentHeap.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBCHComponents(this IServiceCollection services, string subscriptionKey)
    {
        var name = Assembly.GetCallingAssembly().GetName().Name ?? string.Empty;
        
        services.AddScoped<IJSUtilsService, JSUtilsService>();
        services.AddScoped<IUpdateCheckerService, UpdateCheckerService>();
        services.AddScoped<ILocalStorageService, LocalStorageService>();
        services.AddScoped<IHttpService, HttpService>();
        services.AddScoped<IModalService, ModalService>();
        services.AddScoped(_ => new UpdateKeyHolder(subscriptionKey, name));
        services.AddScoped<EncryptProvider>();
        services.AddScoped<SecurityProvider>();

        return services;
    }
}
