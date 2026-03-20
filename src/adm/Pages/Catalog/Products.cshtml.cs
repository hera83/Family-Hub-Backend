using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyHub.Adm.Infrastructure.Clients.Catalog;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Catalog;
using FamilyHub.Adm.Models.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHub.Adm.Pages.Catalog;

public class ProductsModel(ICatalogApiClient catalogApiClient) : PageModel
{
    private readonly ICatalogApiClient _catalogApiClient = catalogApiClient;

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? CategoryId { get; set; }

    public IReadOnlyList<SelectListItem> CategoryOptions { get; private set; } = [];

    public IReadOnlyList<ProductListItemViewModel> Items { get; private set; } = [];

    public string? LoadErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _catalogApiClient.GetCategoriesAsync(cancellationToken);
            CategoryOptions = categories
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToArray();

            var products = await _catalogApiClient.GetProductsAsync(new ProductListQueryRequest
            {
                Search = string.IsNullOrWhiteSpace(Search) ? null : Search.Trim(),
                ItemCategoryId = CategoryId,
                Page = 1,
                PageSize = 200
            }, cancellationToken);

            Items = products.Items
                .Select(x => new ProductListItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    ItemCategoryName = x.ItemCategoryName,
                    Unit = x.Unit,
                    SizeLabel = x.SizeLabel,
                    Price = x.Price,
                    IsFavorite = x.IsFavorite,
                    IsStaple = x.IsStaple
                })
                .ToArray();
        }
        catch (ApiClientException ex)
        {
            LoadErrorMessage = ex.UserMessage;
        }
    }

    public string FormatPrice(decimal? price)
        => price.HasValue ? $"{price.Value:0.##}" : "-";
}
