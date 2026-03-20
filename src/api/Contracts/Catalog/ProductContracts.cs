using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Api.Contracts.Catalog;

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
    [StringLength(200)]
    public string? Search { get; init; }

    public Guid? ItemCategoryId { get; init; }

    public bool FavoritesOnly { get; init; }

    public bool StaplesOnly { get; init; }

    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 200)]
    public int PageSize { get; init; } = 25;
}

public sealed class CreateProductRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    public Guid? ItemCategoryId { get; init; }

    [StringLength(2000)]
    public string? Description { get; init; }

    [StringLength(1000)]
    public string? ImageUrl { get; init; }

    [StringLength(50)]
    public string? Unit { get; init; }

    [StringLength(100)]
    public string? SizeLabel { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? Price { get; init; }

    public bool IsManual { get; init; }

    public bool IsFavorite { get; init; }

    public bool IsStaple { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? CaloriesPer100g { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? FatPer100g { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? CarbsPer100g { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? ProteinPer100g { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? FiberPer100g { get; init; }
}

public sealed class UpdateProductRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    public Guid? ItemCategoryId { get; init; }

    [StringLength(2000)]
    public string? Description { get; init; }

    [StringLength(1000)]
    public string? ImageUrl { get; init; }

    [StringLength(50)]
    public string? Unit { get; init; }

    [StringLength(100)]
    public string? SizeLabel { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? Price { get; init; }

    public bool IsManual { get; init; }

    public bool IsFavorite { get; init; }

    public bool IsStaple { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? CaloriesPer100g { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? FatPer100g { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? CarbsPer100g { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? ProteinPer100g { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? FiberPer100g { get; init; }
}
