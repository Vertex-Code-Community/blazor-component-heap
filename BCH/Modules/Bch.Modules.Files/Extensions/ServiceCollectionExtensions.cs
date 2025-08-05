using Bch.Modules.Files.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bch.Modules.Files.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBchFiles(this IServiceCollection services)
    {
        services.AddScoped<IBchFilesService, BchFilesService>();
        return services;
    }
}