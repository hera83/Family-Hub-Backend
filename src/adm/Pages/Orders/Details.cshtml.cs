using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Orders;
using FamilyHub.Adm.Models.Orders;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Orders;

public class DetailsModel(IOrdersApiClient ordersApiClient) : PageModel
{
    private readonly IOrdersApiClient _ordersApiClient = ordersApiClient;

    public OrderDetailsViewModel? Item { get; private set; }

    public string? LoadErrorMessage { get; private set; }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _ordersApiClient.GetOrderByIdAsync(id, cancellationToken);
            Item = order.ToViewModel();
        }
        catch (ApiClientException ex)
        {
            LoadErrorMessage = ex.UserMessage;
        }
    }

    public string FormatPrice(decimal? price)
        => price.HasValue ? $"{price.Value:0.##}" : "-";

    public string FormatQuantity(decimal? value)
        => value.HasValue ? $"{value.Value:0.###}" : "-";

    public string FormatUtc(DateTime value)
        => value.ToLocalTime().ToString("yyyy-MM-dd HH:mm");

    public string FormatNullableUtc(DateTime? value)
        => value.HasValue ? FormatUtc(value.Value) : "-";
}
