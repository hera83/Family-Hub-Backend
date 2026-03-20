using FamilyHub.Adm.Infrastructure.Clients.Catalog;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Catalog;
using FamilyHub.Adm.Models.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Catalog;

public class CategoryEditModel(ICatalogApiClient catalogApiClient) : PageModel
{
    private readonly ICatalogApiClient _catalogApiClient = catalogApiClient;

    [BindProperty]
    public ItemCategoryEditModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _catalogApiClient.GetCategoryByIdAsync(id, cancellationToken);
            Input = new ItemCategoryEditModel
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

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _catalogApiClient.UpdateCategoryAsync(id, new UpdateItemCategoryRequest
            {
                Name = Input.Name.Trim(),
                SortOrder = Input.SortOrder
            }, cancellationToken);

            TempData["SuccessMessage"] = "Madvarekategori opdateret.";
            return RedirectToPage("/Catalog/Categories");
        }
        catch (ApiClientException ex)
        {
            ModelState.AddModelError(string.Empty, ex.UserMessage);
            return Page();
        }
    }
}
