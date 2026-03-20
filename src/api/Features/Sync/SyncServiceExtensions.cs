using Microsoft.Extensions.DependencyInjection;

namespace FamilyHub.Api.Features.Sync;

public static class SyncServiceExtensions
{
    public static IServiceCollection AddSyncServices(this IServiceCollection services)
    {
        services.AddScoped<ISyncRequestValidator, SyncRequestValidator>();
        services.AddScoped<ISyncService, SyncService>();
        return services;
    }
}
