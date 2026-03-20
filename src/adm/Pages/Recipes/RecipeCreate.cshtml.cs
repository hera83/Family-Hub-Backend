using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Recipes;
using FamilyHub.Adm.Models.Api.Recipes;
using FamilyHub.Adm.Models.Recipes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHub.Adm.Pages.Recipes;

public class RecipeCreateModel(IRecipesApiClient recipesApiClient) : PageModel
{
    private readonly IRecipesApiClient _recipesApiClient = recipesApiClient;

    [BindProperty]
    public RecipeEditModel Input { get; set; } = new();

    public IReadOnlyList<SelectListItem> CategoryOptions { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            await LoadCategoriesAsync(cancellationToken);
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
            var created = await _recipesApiClient.CreateRecipeAsync(ToCreateRequest(Input), cancellationToken);
            TempData["SuccessMessage"] = "Opskrift oprettet.";
            return RedirectToPage("/Recipes/RecipeDetails", new { id = created.Id });
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

    private static CreateRecipeRequest ToCreateRequest(RecipeEditModel input)
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
