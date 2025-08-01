using Microsoft.Extensions.DependencyInjection;
using BlazorComponentHeap.Modal.Services;

namespace BlazorComponentHeap.Modal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBchModal(this IServiceCollection services)
    {
        services.AddScoped<IModalService, ModalService>();
        return services;
    }
}