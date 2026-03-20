using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Api.Contracts.Recipes;

public sealed record RecipeCategoryListItemDto(
    Guid Id,
    string Name,
    int SortOrder
);

public sealed record RecipeCategoryDetailsDto(
    Guid Id,
    string Name,
    int SortOrder,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);

public sealed class CreateRecipeCategoryRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int SortOrder { get; init; }
}

public sealed class UpdateRecipeCategoryRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int SortOrder { get; init; }
}
