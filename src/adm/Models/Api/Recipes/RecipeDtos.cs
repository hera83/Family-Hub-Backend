namespace FamilyHub.Adm.Models.Api.Recipes;

public sealed class RecipeListQueryRequest
{
    public string? Search { get; init; }
    public Guid? RecipeCategoryId { get; init; }
    public bool FavoritesOnly { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 25;
}

public sealed record RecipeListItemDto(
    Guid Id,
    string Title,
    Guid? RecipeCategoryId,
    string? RecipeCategoryName,
    string? ImageUrl,
    int? PrepTimeMinutes,
    int? WaitTimeMinutes,
    bool IsFavorite
);

public sealed record RecipeDetailsDto(
    Guid Id,
    string Title,
    Guid? RecipeCategoryId,
    string? RecipeCategoryName,
    string? ImageUrl,
    string? Description,
    int? PrepTimeMinutes,
    int? WaitTimeMinutes,
    string? Instructions,
    bool IsManual,
    bool IsFavorite,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);

public sealed record RecipeFullDto(
    Guid Id,
    string Title,
    Guid? RecipeCategoryId,
    string? RecipeCategoryName,
    string? ImageUrl,
    string? Description,
    int? PrepTimeMinutes,
    int? WaitTimeMinutes,
    string? Instructions,
    bool IsManual,
    bool IsFavorite,
    IReadOnlyList<RecipeIngredientDto> Ingredients,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);

public sealed class CreateRecipeRequest
{
    public string Title { get; init; } = string.Empty;
    public Guid? RecipeCategoryId { get; init; }
    public string? ImageUrl { get; init; }
    public string? Description { get; init; }
    public int? PrepTimeMinutes { get; init; }
    public int? WaitTimeMinutes { get; init; }
    public string? Instructions { get; init; }
    public bool IsManual { get; init; }
    public bool IsFavorite { get; init; }
}

public sealed class UpdateRecipeRequest
{
    public string Title { get; init; } = string.Empty;
    public Guid? RecipeCategoryId { get; init; }
    public string? ImageUrl { get; init; }
    public string? Description { get; init; }
    public int? PrepTimeMinutes { get; init; }
    public int? WaitTimeMinutes { get; init; }
    public string? Instructions { get; init; }
    public bool IsManual { get; init; }
    public bool IsFavorite { get; init; }
}
