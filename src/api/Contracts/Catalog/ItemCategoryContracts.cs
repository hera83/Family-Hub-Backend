using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Api.Contracts.Catalog;

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
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int SortOrder { get; init; }
}

public sealed class UpdateItemCategoryRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int SortOrder { get; init; }
}
