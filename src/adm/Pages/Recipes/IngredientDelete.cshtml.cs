using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Recipes;
using FamilyHub.Adm.Models.Recipes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Recipes;

public class IngredientDeleteModel(IRecipesApiClient recipesApiClient) : PageModel
{
    private readonly IRecipesApiClient _recipesApiClient = recipesApiClient;

    [BindProperty]
    public RecipeIngredientViewModel? Item { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid recipeId, Guid ingredientId, CancellationToken cancellationToken)
    {
        try
        {
            var ingredients = await _recipesApiClient.GetRecipeIngredientsAsync(recipeId, cancellationToken);
            var ingredient = ingredients.FirstOrDefault(x => x.Id == ingredientId);

            if (ingredient is null)
            {
                TempData["ErrorMessage"] = "Ingrediensen blev ikke fundet.";
                return RedirectToPage("/Recipes/RecipeDetails", new { id = recipeId });
            }

            Item = new RecipeIngredientViewModel
            {
                Id = ingredient.Id,
                RecipeId = ingredient.RecipeId,
                ProductName = ingredient.ProductName,
                ItemCategoryName = ingredient.ItemCategoryName,
                Name = ingredient.Name,
                Quantity = ingredient.Quantity,
                Unit = ingredient.Unit,
                IsStaple = ingredient.IsStaple,
                SortOrder = ingredient.SortOrder
            };

            return Page();
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
            return RedirectToPage("/Recipes/RecipeDetails", new { id = recipeId });
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid recipeId, Guid ingredientId, CancellationToken cancellationToken)
    {
        try
        {
            await _recipesApiClient.DeleteIngredientAsync(recipeId, ingredientId, cancellationToken);
            TempData["SuccessMessage"] = "Ingrediens slettet.";
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
        }

        return RedirectToPage("/Recipes/RecipeDetails", new { id = recipeId });
    }
}
