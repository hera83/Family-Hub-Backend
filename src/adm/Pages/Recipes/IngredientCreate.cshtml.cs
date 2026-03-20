using FamilyHub.Adm.Infrastructure.Clients.Catalog;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Recipes;
using FamilyHub.Adm.Models.Api.Catalog;
using FamilyHub.Adm.Models.Api.Recipes;
using FamilyHub.Adm.Models.Recipes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHub.Adm.Pages.Recipes;

public class IngredientCreateModel(IRecipesApiClient recipesApiClient, ICatalogApiClient catalogApiClient) : PageModel
{
    private readonly IRecipesApiClient _recipesApiClient = recipesApiClient;
    private readonly ICatalogApiClient _catalogApiClient = catalogApiClient;

    [BindProperty]
    public RecipeIngredientEditModel Input { get; set; } = new();

    public IReadOnlyList<SelectListItem> ProductOptions { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(Guid recipeId, CancellationToken cancellationToken)
    {
        Input.RecipeId = recipeId;

        try
        {
            await LoadProductsAsync(cancellationToken);
            return Page();
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
            return RedirectToPage("/Recipes/RecipeDetails", new { id = recipeId });
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid recipeId, CancellationToken cancellationToken)
    {
        Input.RecipeId = recipeId;

        try
        {
            await LoadProductsAsync(cancellationToken);
        }
        catch (ApiClientException ex)
        {
            ModelState.AddModelError(string.Empty, ex.UserMessage);
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _recipesApiClient.AddIngredientAsync(recipeId, new CreateRecipeIngredientRequest
            {
                ProductId = Input.ProductId,
                Name = string.IsNullOrWhiteSpace(Input.Name) ? null : Input.Name.Trim(),
                Quantity = Input.Quantity,
                Unit = string.IsNullOrWhiteSpace(Input.Unit) ? null : Input.Unit.Trim(),
                IsStaple = Input.IsStaple,
                SortOrder = Input.SortOrder
            }, cancellationToken);

            TempData["SuccessMessage"] = "Ingrediens tilfoejet.";
            return RedirectToPage("/Recipes/RecipeDetails", new { id = recipeId });
        }
        catch (ApiClientException ex)
        {
            ModelState.AddModelError(string.Empty, ex.UserMessage);
            return Page();
        }
    }

    private async Task LoadProductsAsync(CancellationToken cancellationToken)
    {
        var products = await _catalogApiClient.GetProductsAsync(new ProductListQueryRequest
        {
            Page = 1,
            PageSize = 250
        }, cancellationToken);

        ProductOptions = products.Items
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem($"{x.Name}{(string.IsNullOrWhiteSpace(x.ItemCategoryName) ? string.Empty : $" ({x.ItemCategoryName})")}", x.Id.ToString()))
            .ToArray();
    }
}
