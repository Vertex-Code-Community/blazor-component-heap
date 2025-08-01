using Microsoft.Extensions.DependencyInjection;
using BlazorComponentHeap.GlobalEvents.Services;

namespace BlazorComponentHeap.GlobalEvents.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBchGlobalEvents(this IServiceCollection services)
    {
        services.AddScoped<IGlobalEventsService, GlobalEventsService>();
        return services;
    }
}