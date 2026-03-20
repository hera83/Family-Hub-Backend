namespace FamilyHub.Adm.Models.Api.Catalog;

public sealed record ProductListItemDto(
    Guid Id,
    string Name,
    Guid? ItemCategoryId,
    string? ItemCategoryName,
    string? ImageUrl,
    string? Unit,
    string? SizeLabel,
    decimal? Price,
    bool IsFavorite,
    bool IsStaple
);

public sealed record ProductDetailsDto(
    Guid Id,
    string Name,
    Guid? ItemCategoryId,
    string? ItemCategoryName,
    string? Description,
    string? ImageUrl,
    string? Unit,
    string? SizeLabel,
    decimal? Price,
    bool IsManual,
    bool IsFavorite,
    bool IsStaple,
    decimal? CaloriesPer100g,
    decimal? FatPer100g,
    decimal? CarbsPer100g,
    decimal? ProteinPer100g,
    decimal? FiberPer100g,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);

public sealed class ProductListQueryRequest
{
    public string? Search { get; init; }
    public Guid? ItemCategoryId { get; init; }
    public bool FavoritesOnly { get; init; }
    public bool StaplesOnly { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 25;
}

public sealed class CreateProductRequest
{
    public string Name { get; init; } = string.Empty;
    public Guid? ItemCategoryId { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public string? Unit { get; init; }
    public string? SizeLabel { get; init; }
    public decimal? Price { get; init; }
    public bool IsManual { get; init; }
    public bool IsFavorite { get; init; }
    public bool IsStaple { get; init; }
    public decimal? CaloriesPer100g { get; init; }
    public decimal? FatPer100g { get; init; }
    public decimal? CarbsPer100g { get; init; }
    public decimal? ProteinPer100g { get; init; }
    public decimal? FiberPer100g { get; init; }
}

public sealed class UpdateProductRequest
{
    public string Name { get; init; } = string.Empty;
    public Guid? ItemCategoryId { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public string? Unit { get; init; }
    public string? SizeLabel { get; init; }
    public decimal? Price { get; init; }
    public bool IsManual { get; init; }
    public bool IsFavorite { get; init; }
    public bool IsStaple { get; init; }
    public decimal? CaloriesPer100g { get; init; }
    public decimal? FatPer100g { get; init; }
    public decimal? CarbsPer100g { get; init; }
    public decimal? ProteinPer100g { get; init; }
    public decimal? FiberPer100g { get; init; }
}
