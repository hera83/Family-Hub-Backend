using FamilyHub.Api.Contracts.Orders;

namespace FamilyHub.Api.Features.Orders;

public interface IOrderSyncService
{
    Task<OrderSyncDto> GetCurrentStateAsync(CancellationToken ct = default);
    Task<OrderChangesSinceDto> GetChangesSinceAsync(string? sinceUtc, CancellationToken ct = default);
}
