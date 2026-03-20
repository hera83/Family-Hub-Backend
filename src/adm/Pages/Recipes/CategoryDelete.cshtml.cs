using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Recipes;
using FamilyHub.Adm.Models.Recipes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Recipes;

public class CategoryDeleteModel(IRecipesApiClient recipesApiClient) : PageModel
{
    private readonly IRecipesApiClient _recipesApiClient = recipesApiClient;

    [BindProperty]
    public RecipeCategoryListItemViewModel? Item { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _recipesApiClient.GetCategoryByIdAsync(id, cancellationToken);
            Item = new RecipeCategoryListItemViewModel
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
            return RedirectToPage("/Recipes/Categories");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Item is null || Item.Id == Guid.Empty)
        {
            TempData["ErrorMessage"] = "Ugyldig opskriftskategori.";
            return RedirectToPage("/Recipes/Categories");
        }

        try
        {
            await _recipesApiClient.DeleteCategoryAsync(Item.Id, cancellationToken);
            TempData["SuccessMessage"] = "Opskriftskategori slettet.";
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
        }

        return RedirectToPage("/Recipes/Categories");
    }
}
