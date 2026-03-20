using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Orders;
using FamilyHub.Adm.Models.Api.Orders;
using FamilyHub.Adm.Models.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHub.Adm.Pages.Orders;

public class IndexModel(IOrdersApiClient ordersApiClient) : PageModel
{
    private readonly IOrdersApiClient _ordersApiClient = ordersApiClient;

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    public IReadOnlyList<SelectListItem> StatusOptions { get; private set; } =
    [
        new("Created", "Created"),
        new("Confirmed", "Confirmed"),
        new("Completed", "Completed"),
        new("Cancelled", "Cancelled")
    ];

    public IReadOnlyList<OrderListItemViewModel> Items { get; private set; } = [];

    public string? LoadErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var orders = await _ordersApiClient.GetOrdersAsync(new OrderListQueryRequest
            {
                Status = string.IsNullOrWhiteSpace(Status) ? null : Status.Trim(),
                Page = 1,
                PageSize = 200
            }, cancellationToken);

            Items = orders.Items
                .Select(x => x.ToViewModel())
                .ToArray();
        }
        catch (ApiClientException ex)
        {
            LoadErrorMessage = ex.UserMessage;
        }
    }

    public string FormatPrice(decimal? price)
        => price.HasValue ? $"{price.Value:0.##}" : "-";

    public string FormatUtc(DateTime value)
        => value.ToLocalTime().ToString("yyyy-MM-dd HH:mm");

    public string FormatNullableUtc(DateTime? value)
        => value.HasValue ? FormatUtc(value.Value) : "-";

    public string ShortId(Guid id)
        => id.ToString("N")[..8];
}
