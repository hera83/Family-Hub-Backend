using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Recipes;
using FamilyHub.Adm.Models.Recipes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Recipes;

public class RecipeDeleteModel(IRecipesApiClient recipesApiClient) : PageModel
{
    private readonly IRecipesApiClient _recipesApiClient = recipesApiClient;

    [BindProperty]
    public RecipeListItemViewModel? Item { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await _recipesApiClient.GetRecipeByIdAsync(id, cancellationToken);
            Item = new RecipeListItemViewModel
            {
                Id = recipe.Id,
                Title = recipe.Title,
                RecipeCategoryName = recipe.RecipeCategoryName,
                PrepTimeMinutes = recipe.PrepTimeMinutes,
                WaitTimeMinutes = recipe.WaitTimeMinutes,
                IsFavorite = recipe.IsFavorite
            };

            return Page();
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
            return RedirectToPage("/Recipes/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Item is null || Item.Id == Guid.Empty)
        {
            TempData["ErrorMessage"] = "Ugyldig opskrift.";
            return RedirectToPage("/Recipes/Index");
        }

        try
        {
            await _recipesApiClient.DeleteRecipeAsync(Item.Id, cancellationToken);
            TempData["SuccessMessage"] = "Opskrift slettet.";
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
        }

        return RedirectToPage("/Recipes/Index");
    }
}
