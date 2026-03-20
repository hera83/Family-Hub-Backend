using FamilyHub.Adm.Infrastructure.Clients.Catalog;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Catalog;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Catalog;

public class ProductDetailsModel(ICatalogApiClient catalogApiClient) : PageModel
{
    private readonly ICatalogApiClient _catalogApiClient = catalogApiClient;

    public ProductEditModel? Product { get; private set; }

    public string? LoadErrorMessage { get; private set; }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _catalogApiClient.GetProductByIdAsync(id, cancellationToken);
            Product = new ProductEditModel
            {
                Id = dto.Id,
                Name = dto.Name,
                ItemCategoryId = dto.ItemCategoryId,
                ItemCategoryName = dto.ItemCategoryName,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Unit = dto.Unit,
                SizeLabel = dto.SizeLabel,
                Price = dto.Price,
                IsManual = dto.IsManual,
                IsFavorite = dto.IsFavorite,
                IsStaple = dto.IsStaple,
                CaloriesPer100g = dto.CaloriesPer100g,
                FatPer100g = dto.FatPer100g,
                CarbsPer100g = dto.CarbsPer100g,
                ProteinPer100g = dto.ProteinPer100g,
                FiberPer100g = dto.FiberPer100g
            };
        }
        catch (ApiClientException ex)
        {
            LoadErrorMessage = ex.UserMessage;
        }
    }

    public string FormatPrice(decimal? price)
        => price.HasValue ? $"{price.Value:0.##}" : "-";
}
