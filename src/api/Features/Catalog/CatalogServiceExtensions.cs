using Microsoft.Extensions.DependencyInjection;

namespace FamilyHub.Api.Features.Catalog;

public static class CatalogServiceExtensions
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services)
    {
        services.AddScoped<IItemCategoryRequestValidator, ItemCategoryRequestValidator>();
        services.AddScoped<IProductRequestValidator, ProductRequestValidator>();
        services.AddScoped<ICatalogSyncRequestValidator, CatalogSyncRequestValidator>();
        services.AddScoped<IItemCategoryService, ItemCategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICatalogSyncService, CatalogSyncService>();
        return services;
    }
}
