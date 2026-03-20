using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Common;
using FamilyHub.Adm.Models.Api.Orders;

namespace FamilyHub.Adm.Infrastructure.Clients.Orders;

public sealed class OrdersApiClient(HttpClient httpClient) : ApiClientBase(httpClient), IOrdersApiClient
{
    public Task<PagedListResponse<OrderListItemDto>> GetOrdersAsync(OrderListQueryRequest? query = null, CancellationToken cancellationToken = default)
    {
        var effectiveQuery = query ?? new OrderListQueryRequest();

        var uri = WithQueryString("api/v1/orders", new Dictionary<string, string?>
        {
            ["status"] = effectiveQuery.Status,
            ["page"] = effectiveQuery.Page.ToString(),
            ["pageSize"] = effectiveQuery.PageSize.ToString()
        });

        return GetAsync<PagedListResponse<OrderListItemDto>>(uri, cancellationToken);
    }

    public Task<OrderDetailsDto> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => GetAsync<OrderDetailsDto>($"api/v1/orders/{id}", cancellationToken);

    public Task<OrderPdfDto> GetOrderPdfAsync(Guid id, CancellationToken cancellationToken = default)
        => GetAsync<OrderPdfDto>($"api/v1/orders/{id}/pdf", cancellationToken);

    public Task DeleteOrderAsync(Guid id, CancellationToken cancellationToken = default)
        => DeleteAsync($"api/v1/orders/{id}", cancellationToken);
}
