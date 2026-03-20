namespace FamilyHub.Adm.Models.Api.Recipes;

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
    public string Name { get; init; } = string.Empty;
    public int SortOrder { get; init; }
}

public sealed class UpdateRecipeCategoryRequest
{
    public string Name { get; init; } = string.Empty;
    public int SortOrder { get; init; }
}
