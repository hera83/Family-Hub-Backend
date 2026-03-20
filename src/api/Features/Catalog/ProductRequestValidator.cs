using FamilyHub.Api.Contracts.Catalog;
using FamilyHub.Api.Features.Common;

namespace FamilyHub.Api.Features.Catalog;

internal sealed class ProductRequestValidator : IProductRequestValidator
{
    public void Validate(ProductListQueryRequest request)
        => PaginationValidator.Validate(request.Page, request.PageSize);

    public void Validate(CreateProductRequest request)
        => ValidateCore(
            request.Name,
            request.Price,
            request.CaloriesPer100g,
            request.FatPer100g,
            request.CarbsPer100g,
            request.ProteinPer100g,
            request.FiberPer100g);

    public void Validate(UpdateProductRequest request)
        => ValidateCore(
            request.Name,
            request.Price,
            request.CaloriesPer100g,
            request.FatPer100g,
            request.CarbsPer100g,
            request.ProteinPer100g,
            request.FiberPer100g);

    private static void ValidateCore(
        string? name,
        decimal? price,
        decimal? caloriesPer100g,
        decimal? fatPer100g,
        decimal? carbsPer100g,
        decimal? proteinPer100g,
        decimal? fiberPer100g)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name er påkrævet.");

        if (price < 0)
            throw new ArgumentException("Price må ikke være negativ.");

        if (caloriesPer100g < 0 || fatPer100g < 0 || carbsPer100g < 0 || proteinPer100g < 0 || fiberPer100g < 0)
            throw new ArgumentException("Næringsværdier må ikke være negative.");
    }
}
