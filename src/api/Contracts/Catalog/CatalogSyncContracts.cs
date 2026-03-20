namespace FamilyHub.Api.Contracts.Catalog;

public sealed record CatalogSyncDto(
    DateTime GeneratedAtUtc,
    IReadOnlyList<ItemCategoryDetailsDto> ItemCategories,
    IReadOnlyList<ProductDetailsDto> Products
);

public sealed record CatalogChangesSinceDto(
    DateTime SinceUtc,
    DateTime GeneratedAtUtc,
    IReadOnlyList<ItemCategoryDetailsDto> ItemCategories,
    IReadOnlyList<ProductDetailsDto> Products
);
