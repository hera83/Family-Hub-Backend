using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Recipes;
using FamilyHub.Adm.Models.Recipes;

namespace FamilyHub.Adm.Pages.Recipes;

public class CategoriesModel(IRecipesApiClient recipesApiClient) : PageModel
{
    private readonly IRecipesApiClient _recipesApiClient = recipesApiClient;

    public IReadOnlyList<RecipeCategoryListItemViewModel> Items { get; private set; } = [];

    public string? LoadErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _recipesApiClient.GetCategoriesAsync(cancellationToken);
            Items = categories
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .Select(x => new RecipeCategoryListItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    SortOrder = x.SortOrder
                })
                .ToArray();
        }
        catch (ApiClientException ex)
        {
            LoadErrorMessage = ex.UserMessage;
        }
    }
}
