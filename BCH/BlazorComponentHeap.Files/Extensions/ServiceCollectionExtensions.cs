using Microsoft.Extensions.DependencyInjection;
using BlazorComponentHeap.Files.Services;

namespace BlazorComponentHeap.Files.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBchFiles(this IServiceCollection services)
    {
        services.AddScoped<IBchFilesService, BchFilesService>();
        return services;
    }
}