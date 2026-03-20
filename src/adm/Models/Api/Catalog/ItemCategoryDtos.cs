namespace FamilyHub.Adm.Models.Api.Catalog;

public sealed record ItemCategoryListItemDto(
    Guid Id,
    string Name,
    int SortOrder
);

public sealed record ItemCategoryDetailsDto(
    Guid Id,
    string Name,
    int SortOrder,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);

public sealed class CreateItemCategoryRequest
{
    public string Name { get; init; } = string.Empty;
    public int SortOrder { get; init; }
}

public sealed class UpdateItemCategoryRequest
{
    public string Name { get; init; } = string.Empty;
    public int SortOrder { get; init; }
}
