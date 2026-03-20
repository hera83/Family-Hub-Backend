using FamilyHub.Adm.Infrastructure.Clients.Catalog;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Catalog;

public class CategoryDeleteModel(ICatalogApiClient catalogApiClient) : PageModel
{
    private readonly ICatalogApiClient _catalogApiClient = catalogApiClient;

    [BindProperty]
    public ItemCategoryListItemViewModel? Item { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _catalogApiClient.GetCategoryByIdAsync(id, cancellationToken);
            Item = new ItemCategoryListItemViewModel
            {
                Id = category.Id,
                Name = category.Name,
                SortOrder = category.SortOrder
            };

            return Page();
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
            return RedirectToPage("/Catalog/Categories");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Item is null || Item.Id == Guid.Empty)
        {
            TempData["ErrorMessage"] = "Ugyldig kategori.";
            return RedirectToPage("/Catalog/Categories");
        }

        try
        {
            await _catalogApiClient.DeleteCategoryAsync(Item.Id, cancellationToken);
            TempData["SuccessMessage"] = "Madvarekategori slettet.";
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
        }

        return RedirectToPage("/Catalog/Categories");
    }
}
