using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using BlazorComponentHeap.Core.Services;
using BlazorComponentHeap.Core.Services.Interfaces;

namespace BlazorComponentHeap.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBCHComponents(this IServiceCollection services)
    {
        var name = Assembly.GetCallingAssembly().GetName().Name ?? string.Empty;
        
        services.AddScoped<IJSUtilsService, JSUtilsService>();
        services.AddScoped<ILocalStorageService, LocalStorageService>();
        services.AddScoped<IModalService, ModalService>();

        return services;
    }
}
