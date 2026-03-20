using FamilyHub.Api.Contracts.Common;
using FamilyHub.Api.Contracts.Orders;

namespace FamilyHub.Api.Features.Orders;

public interface IOrderService
{
    Task<PagedListResponse<OrderListItemDto>> GetAllAsync(OrderListQueryRequest query, CancellationToken ct = default);
    Task<OrderDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<OrderPdfDto?> GetPdfByOrderIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
