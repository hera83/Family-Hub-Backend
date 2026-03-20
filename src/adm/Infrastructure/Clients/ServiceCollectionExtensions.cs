using FamilyHub.Adm.Configuration;
using FamilyHub.Adm.Infrastructure.Clients.Calendar;
using FamilyHub.Adm.Infrastructure.Clients.Catalog;
using FamilyHub.Adm.Infrastructure.Clients.Orders;
using FamilyHub.Adm.Infrastructure.Clients.Recipes;
using FamilyHub.Adm.Infrastructure.Clients.Sync;
using Microsoft.Extensions.Options;

namespace FamilyHub.Adm.Infrastructure.Clients;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFamilyHubApiClients(this IServiceCollection services)
    {
        services.AddTransient<ApiKeyHeaderHandler>();

        services.AddFamilyHubHttpClient<ICalendarApiClient, CalendarApiClient>();
        services.AddFamilyHubHttpClient<ICatalogApiClient, CatalogApiClient>();
        services.AddFamilyHubHttpClient<IOrdersApiClient, OrdersApiClient>();
        services.AddFamilyHubHttpClient<IRecipesApiClient, RecipesApiClient>();
        services.AddFamilyHubHttpClient<ISyncApiClient, SyncApiClient>();

        return services;
    }

    private static void AddFamilyHubHttpClient<TClient, TImplementation>(this IServiceCollection services)
        where TClient : class
        where TImplementation : class, TClient
    {
        services
            .AddHttpClient<TClient, TImplementation>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<FamilyHubApiOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
            })
            .AddHttpMessageHandler<ApiKeyHeaderHandler>();
    }
}
