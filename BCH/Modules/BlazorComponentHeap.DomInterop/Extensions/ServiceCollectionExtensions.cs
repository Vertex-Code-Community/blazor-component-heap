using Microsoft.Extensions.DependencyInjection;
using BlazorComponentHeap.DomInterop.Services;

namespace BlazorComponentHeap.DomInterop.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBchDomInterop(this IServiceCollection services)
    {
        services.AddScoped<IDomInteropService, DomInteropService>();
        return services;
    }
}