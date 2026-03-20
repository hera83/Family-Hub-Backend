using FamilyHub.Adm.Infrastructure.Clients.Catalog;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Catalog;
using FamilyHub.Adm.Models.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHub.Adm.Pages.Catalog;

public class ProductCreateModel(ICatalogApiClient catalogApiClient) : PageModel
{
    private readonly ICatalogApiClient _catalogApiClient = catalogApiClient;

    [BindProperty]
    public ProductEditModel Input { get; set; } = new();

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
            return RedirectToPage("/Catalog/Products");
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
            await _catalogApiClient.CreateProductAsync(ToCreateRequest(Input), cancellationToken);
            TempData["SuccessMessage"] = "Produkt oprettet.";
            return RedirectToPage("/Catalog/Products");
        }
        catch (ApiClientException ex)
        {
            ModelState.AddModelError(string.Empty, ex.UserMessage);
            return Page();
        }
    }

    private async Task LoadCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = await _catalogApiClient.GetCategoriesAsync(cancellationToken);
        CategoryOptions = categories
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToArray();
    }

    private static CreateProductRequest ToCreateRequest(ProductEditModel input)
        => new()
        {
            Name = input.Name.Trim(),
            ItemCategoryId = input.ItemCategoryId,
            Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim(),
            ImageUrl = string.IsNullOrWhiteSpace(input.ImageUrl) ? null : input.ImageUrl.Trim(),
            Unit = string.IsNullOrWhiteSpace(input.Unit) ? null : input.Unit.Trim(),
            SizeLabel = string.IsNullOrWhiteSpace(input.SizeLabel) ? null : input.SizeLabel.Trim(),
            Price = input.Price,
            IsManual = input.IsManual,
            IsFavorite = input.IsFavorite,
            IsStaple = input.IsStaple,
            CaloriesPer100g = input.CaloriesPer100g,
            FatPer100g = input.FatPer100g,
            CarbsPer100g = input.CarbsPer100g,
            ProteinPer100g = input.ProteinPer100g,
            FiberPer100g = input.FiberPer100g
        };
}
