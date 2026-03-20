using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Recipes;
using FamilyHub.Adm.Models.Api.Recipes;
using FamilyHub.Adm.Models.Recipes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHub.Adm.Pages.Recipes;

public class IndexModel(IRecipesApiClient recipesApiClient) : PageModel
{
    private readonly IRecipesApiClient _recipesApiClient = recipesApiClient;

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? CategoryId { get; set; }

    public IReadOnlyList<SelectListItem> CategoryOptions { get; private set; } = [];

    public IReadOnlyList<RecipeListItemViewModel> Items { get; private set; } = [];

    public string? LoadErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _recipesApiClient.GetCategoriesAsync(cancellationToken);
            CategoryOptions = categories
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToArray();

            var recipes = await _recipesApiClient.GetRecipesAsync(new RecipeListQueryRequest
            {
                Search = string.IsNullOrWhiteSpace(Search) ? null : Search.Trim(),
                RecipeCategoryId = CategoryId,
                Page = 1,
                PageSize = 200
            }, cancellationToken);

            Items = recipes.Items
                .Select(x => new RecipeListItemViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    RecipeCategoryName = x.RecipeCategoryName,
                    PrepTimeMinutes = x.PrepTimeMinutes,
                    WaitTimeMinutes = x.WaitTimeMinutes,
                    IsFavorite = x.IsFavorite
                })
                .ToArray();
        }
        catch (ApiClientException ex)
        {
            LoadErrorMessage = ex.UserMessage;
        }
    }
}
