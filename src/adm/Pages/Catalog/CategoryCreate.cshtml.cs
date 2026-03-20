using FamilyHub.Adm.Infrastructure.Clients.Catalog;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Catalog;
using FamilyHub.Adm.Models.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Catalog;

public class CategoryCreateModel(ICatalogApiClient catalogApiClient) : PageModel
{
    private readonly ICatalogApiClient _catalogApiClient = catalogApiClient;

    [BindProperty]
    public ItemCategoryEditModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _catalogApiClient.CreateCategoryAsync(new CreateItemCategoryRequest
            {
                Name = Input.Name.Trim(),
                SortOrder = Input.SortOrder
            }, cancellationToken);

            TempData["SuccessMessage"] = "Madvarekategori oprettet.";
            return RedirectToPage("/Catalog/Categories");
        }
        catch (ApiClientException ex)
        {
            ModelState.AddModelError(string.Empty, ex.UserMessage);
            return Page();
        }
    }
}
