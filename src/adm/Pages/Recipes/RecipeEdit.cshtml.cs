using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Recipes;
using FamilyHub.Adm.Models.Api.Recipes;
using FamilyHub.Adm.Models.Recipes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHub.Adm.Pages.Recipes;

public class RecipeEditPageModel(IRecipesApiClient recipesApiClient) : PageModel
{
    private readonly IRecipesApiClient _recipesApiClient = recipesApiClient;

    [BindProperty]
    public RecipeEditModel Input { get; set; } = new();

    public IReadOnlyList<SelectListItem> CategoryOptions { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await _recipesApiClient.GetRecipeByIdAsync(id, cancellationToken);
            Input = new RecipeEditModel
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
            };

            await LoadCategoriesAsync(cancellationToken);
            return Page();
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
            return RedirectToPage("/Recipes/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await LoadCategoriesAsync(cancellationToken);
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
            await _recipesApiClient.UpdateRecipeAsync(id, ToUpdateRequest(Input), cancellationToken);
            TempData["SuccessMessage"] = "Opskrift opdateret.";
            return RedirectToPage("/Recipes/RecipeDetails", new { id });
        }
        catch (ApiClientException ex)
        {
            ModelState.AddModelError(string.Empty, ex.UserMessage);
            return Page();
        }
    }

    private async Task LoadCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = await _recipesApiClient.GetCategoriesAsync(cancellationToken);
        CategoryOptions = categories
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToArray();
    }

    private static UpdateRecipeRequest ToUpdateRequest(RecipeEditModel input)
        => new()
        {
            Title = input.Title.Trim(),
            RecipeCategoryId = input.RecipeCategoryId,
            ImageUrl = string.IsNullOrWhiteSpace(input.ImageUrl) ? null : input.ImageUrl.Trim(),
            Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim(),
            PrepTimeMinutes = input.PrepTimeMinutes,
            WaitTimeMinutes = input.WaitTimeMinutes,
            Instructions = string.IsNullOrWhiteSpace(input.Instructions) ? null : input.Instructions.Trim(),
            IsManual = input.IsManual,
            IsFavorite = input.IsFavorite
        };
}
