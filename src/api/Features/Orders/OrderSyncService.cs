using FamilyHub.Api.Contracts.Orders;
using FamilyHub.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Features.Orders;

public sealed class OrderSyncService(
    FamilyHubDbContext db,
    IOrderSyncRequestValidator validator) : IOrderSyncService
{
    public async Task<OrderSyncDto> GetCurrentStateAsync(CancellationToken ct = default)
    {
        var orders = (await db.Orders
            .AsNoTracking()
            .Include(o => o.Lines)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(ct))
            .Select(o => o.ToDetailsDto())
            .ToList();

        var orderLines = orders.SelectMany(o => o.Lines).ToList();

        return new OrderSyncDto(DateTime.UtcNow, orders, orderLines);
    }

    public async Task<OrderChangesSinceDto> GetChangesSinceAsync(string? sinceUtc, CancellationToken ct = default)
    {
        var sinceUtcValue = validator.ValidateAndParseSinceUtc(sinceUtc);

        var orders = (await db.Orders
            .AsNoTracking()
            .Include(o => o.Lines)
            .Where(o => (o.UpdatedAtUtc ?? o.CreatedAtUtc) > sinceUtcValue)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(ct))
            .Select(o => o.ToDetailsDto())
            .ToList();

        var orderLines = orders.SelectMany(o => o.Lines).ToList();

        return new OrderChangesSinceDto(sinceUtcValue, DateTime.UtcNow, orders, orderLines);
    }
}
