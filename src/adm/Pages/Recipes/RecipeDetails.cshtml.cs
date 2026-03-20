using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Recipes;
using FamilyHub.Adm.Models.Recipes;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Recipes;

public class RecipeDetailsModel(IRecipesApiClient recipesApiClient) : PageModel
{
    private readonly IRecipesApiClient _recipesApiClient = recipesApiClient;

    public RecipeFullViewModel View { get; private set; } = new();

    public string? LoadErrorMessage { get; private set; }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await _recipesApiClient.GetRecipeFullAsync(id, cancellationToken);
            View = new RecipeFullViewModel
            {
                Recipe = new RecipeEditModel
                {
                    Id = recipe.Id,
                    Title = recipe.Title,
                    RecipeCategoryId = recipe.RecipeCategoryId,
                    RecipeCategoryName = recipe.RecipeCategoryName,
                    ImageUrl = recipe.ImageUrl,
                    Description = recipe.Description,
                    PrepTimeMinutes = recipe.PrepTimeMinutes,
                    WaitTimeMinutes = recipe.WaitTimeMinutes,
                    Instructions = recipe.Instructions,
                    IsManual = recipe.IsManual,
                    IsFavorite = recipe.IsFavorite
                },
                Ingredients = recipe.Ingredients
                    .Select(x => new RecipeIngredientViewModel
                    {
                        Id = x.Id,
                        RecipeId = x.RecipeId,
                        ProductName = x.ProductName,
                        ItemCategoryName = x.ItemCategoryName,
                        Name = x.Name,
                        Quantity = x.Quantity,
                        Unit = x.Unit,
                        IsStaple = x.IsStaple,
                        SortOrder = x.SortOrder
                    })
                    .ToArray()
            };
        }
        catch (ApiClientException ex)
        {
            LoadErrorMessage = ex.UserMessage;
        }
    }
}
