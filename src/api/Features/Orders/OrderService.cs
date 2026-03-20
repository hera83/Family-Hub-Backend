using FamilyHub.Api.Contracts.Common;
using FamilyHub.Api.Contracts.Orders;
using FamilyHub.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Features.Orders;

public sealed class OrderService(
    FamilyHubDbContext db,
    IOrderRequestValidator validator) : IOrderService
{
    public async Task<PagedListResponse<OrderListItemDto>> GetAllAsync(OrderListQueryRequest query, CancellationToken ct = default)
    {
        validator.Validate(query);

        var q = db.Orders
            .Include(o => o.Lines)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Status))
            q = q.Where(o => o.Status == query.Status);

        var totalCount = await q.CountAsync(ct);

        var orders = await q
            .OrderByDescending(o => o.CreatedAtUtc)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return new PagedListResponse<OrderListItemDto>
        {
            Items      = orders.Select(o => o.ToListItemDto()).ToList(),
            Page       = query.Page,
            PageSize   = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<OrderDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        return order?.ToDetailsDto();
    }

    public async Task<OrderPdfDto?> GetPdfByOrderIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await db.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        return order?.ToPdfDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var order = await db.Orders.FindAsync([id], ct);
        if (order is null) return false;

        db.Orders.Remove(order);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
