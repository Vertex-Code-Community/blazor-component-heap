using Microsoft.Extensions.DependencyInjection;
using BlazorComponentHeap.Storage.Services;

namespace BlazorComponentHeap.Storage.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBchStorage(this IServiceCollection services)
    {
        services.AddScoped<ILocalStorageService, LocalStorageService>();
        return services;
    }
}