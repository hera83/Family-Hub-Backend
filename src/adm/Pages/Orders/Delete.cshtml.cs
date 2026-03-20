using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Orders;
using FamilyHub.Adm.Models.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Orders;

public class DeleteModel(IOrdersApiClient ordersApiClient) : PageModel
{
    private readonly IOrdersApiClient _ordersApiClient = ordersApiClient;

    [BindProperty]
    public OrderListItemViewModel? Item { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _ordersApiClient.GetOrderByIdAsync(id, cancellationToken);
            Item = new OrderListItemViewModel
            {
                Id = order.Id,
                Status = order.Status,
                TotalItems = order.TotalItems,
                TotalPrice = order.TotalPrice,
                HasPdf = order.HasPdf,
                CreatedAtUtc = order.CreatedAtUtc,
                UpdatedAtUtc = order.UpdatedAtUtc
            };

            return Page();
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
            return RedirectToPage("/Orders/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Item is null || Item.Id == Guid.Empty)
        {
            TempData["ErrorMessage"] = "Ugyldig ordre.";
            return RedirectToPage("/Orders/Index");
        }

        try
        {
            await _ordersApiClient.DeleteOrderAsync(Item.Id, cancellationToken);
            TempData["SuccessMessage"] = "Ordre slettet.";
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
        }

        return RedirectToPage("/Orders/Index");
    }

    public string FormatPrice(decimal? price)
        => price.HasValue ? $"{price.Value:0.##}" : "-";
}
