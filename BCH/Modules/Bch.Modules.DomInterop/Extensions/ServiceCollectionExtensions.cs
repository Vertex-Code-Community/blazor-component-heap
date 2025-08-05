using Bch.Modules.DomInterop.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bch.Modules.DomInterop.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBchDomInterop(this IServiceCollection services)
    {
        services.AddScoped<IDomInteropService, DomInteropService>();
        return services;
    }
}