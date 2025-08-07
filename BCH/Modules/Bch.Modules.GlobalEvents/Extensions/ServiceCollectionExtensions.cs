using Microsoft.Extensions.DependencyInjection;
using Bch.Modules.GlobalEvents.Services;

namespace Bch.Modules.GlobalEvents.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBchGlobalEvents(this IServiceCollection services)
    {
        services.AddScoped<IGlobalEventsService, GlobalEventsService>();
        return services;
    }
}