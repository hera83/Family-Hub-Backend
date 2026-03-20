using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyHub.Adm.Infrastructure.Clients.Catalog;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Catalog;

namespace FamilyHub.Adm.Pages.Catalog;

public class CategoriesModel(ICatalogApiClient catalogApiClient) : PageModel
{
    private readonly ICatalogApiClient _catalogApiClient = catalogApiClient;

    public IReadOnlyList<ItemCategoryListItemViewModel> Items { get; private set; } = [];

    public string? LoadErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _catalogApiClient.GetCategoriesAsync(cancellationToken);
            Items = categories
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .Select(x => new ItemCategoryListItemViewModel
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
