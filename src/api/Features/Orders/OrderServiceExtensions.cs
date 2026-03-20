using Microsoft.Extensions.DependencyInjection;

namespace FamilyHub.Api.Features.Orders;

public static class OrderServiceExtensions
{
    public static IServiceCollection AddOrderServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderRequestValidator, OrderRequestValidator>();
        services.AddScoped<IOrderSyncRequestValidator, OrderSyncRequestValidator>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderSyncService, OrderSyncService>();
        return services;
    }
}
