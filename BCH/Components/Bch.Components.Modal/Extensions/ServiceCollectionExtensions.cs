using Microsoft.Extensions.DependencyInjection;
using Bch.Components.Modal.Services;

namespace Bch.Components.Modal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBchModal(this IServiceCollection services)
    {
        services.AddScoped<IModalService, ModalService>();
        return services;
    }
}