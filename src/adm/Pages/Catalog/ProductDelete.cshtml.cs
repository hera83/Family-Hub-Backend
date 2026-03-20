using FamilyHub.Adm.Infrastructure.Clients.Catalog;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Catalog;

public class ProductDeleteModel(ICatalogApiClient catalogApiClient) : PageModel
{
    private readonly ICatalogApiClient _catalogApiClient = catalogApiClient;

    [BindProperty]
    public ProductListItemViewModel? Item { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _catalogApiClient.GetProductByIdAsync(id, cancellationToken);
            Item = new ProductListItemViewModel
            {
                Id = product.Id,
                Name = product.Name,
                ItemCategoryName = product.ItemCategoryName,
                Unit = product.Unit,
                SizeLabel = product.SizeLabel,
                Price = product.Price,
                IsFavorite = product.IsFavorite,
                IsStaple = product.IsStaple
            };

            return Page();
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
            return RedirectToPage("/Catalog/Products");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Item is null || Item.Id == Guid.Empty)
        {
            TempData["ErrorMessage"] = "Ugyldigt produkt.";
            return RedirectToPage("/Catalog/Products");
        }

        try
        {
            await _catalogApiClient.DeleteProductAsync(Item.Id, cancellationToken);
            TempData["SuccessMessage"] = "Produkt slettet.";
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
        }

        return RedirectToPage("/Catalog/Products");
    }

    public string FormatPrice(decimal? price)
        => price.HasValue ? $"{price.Value:0.##}" : "-";
}
