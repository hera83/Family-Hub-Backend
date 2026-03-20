using FamilyHub.Adm.Models.Api.Common;
using FamilyHub.Adm.Models.Api.Orders;

namespace FamilyHub.Adm.Infrastructure.Clients.Orders;

public interface IOrdersApiClient
{
    Task<PagedListResponse<OrderListItemDto>> GetOrdersAsync(OrderListQueryRequest? query = null, CancellationToken cancellationToken = default);
    Task<OrderDetailsDto> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrderPdfDto> GetOrderPdfAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteOrderAsync(Guid id, CancellationToken cancellationToken = default);
}
