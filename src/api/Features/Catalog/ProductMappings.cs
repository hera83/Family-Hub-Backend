using FamilyHub.Api.Contracts.Catalog;
using FamilyHub.Api.Entities.Catalog;

namespace FamilyHub.Api.Features.Catalog;

internal static class ProductMappings
{
    internal static ProductListItemDto ToListItemDto(this Product product) => new(
        product.Id,
        product.Name,
        product.ItemCategoryId,
        product.ItemCategory?.Name,
        product.ImageUrl,
        product.Unit,
        product.SizeLabel,
        product.Price,
        product.IsFavorite,
        product.IsStaple);

    internal static ProductDetailsDto ToDetailsDto(this Product product) => new(
        product.Id,
        product.Name,
        product.ItemCategoryId,
        product.ItemCategory?.Name,
        product.Description,
        product.ImageUrl,
        product.Unit,
        product.SizeLabel,
        product.Price,
        product.IsManual,
        product.IsFavorite,
        product.IsStaple,
        product.CaloriesPer100g,
        product.FatPer100g,
        product.CarbsPer100g,
        product.ProteinPer100g,
        product.FiberPer100g,
        product.CreatedAtUtc,
        product.UpdatedAtUtc);

    internal static Product ToEntity(this CreateProductRequest request) => new()
    {
        Name = request.Name,
        ItemCategoryId = request.ItemCategoryId,
        Description = request.Description,
        ImageUrl = request.ImageUrl,
        Unit = request.Unit,
        SizeLabel = request.SizeLabel,
        Price = request.Price,
        IsManual = request.IsManual,
        IsFavorite = request.IsFavorite,
        IsStaple = request.IsStaple,
        CaloriesPer100g = request.CaloriesPer100g,
        FatPer100g = request.FatPer100g,
        CarbsPer100g = request.CarbsPer100g,
        ProteinPer100g = request.ProteinPer100g,
        FiberPer100g = request.FiberPer100g
    };

    internal static void Apply(this Product product, UpdateProductRequest request)
    {
        product.Name = request.Name;
        product.ItemCategoryId = request.ItemCategoryId;
        product.Description = request.Description;
        product.ImageUrl = request.ImageUrl;
        product.Unit = request.Unit;
        product.SizeLabel = request.SizeLabel;
        product.Price = request.Price;
        product.IsManual = request.IsManual;
        product.IsFavorite = request.IsFavorite;
        product.IsStaple = request.IsStaple;
        product.CaloriesPer100g = request.CaloriesPer100g;
        product.FatPer100g = request.FatPer100g;
        product.CarbsPer100g = request.CarbsPer100g;
        product.ProteinPer100g = request.ProteinPer100g;
        product.FiberPer100g = request.FiberPer100g;
    }
}