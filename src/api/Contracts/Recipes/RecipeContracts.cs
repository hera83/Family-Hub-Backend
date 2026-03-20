using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Api.Contracts.Recipes;

public sealed class RecipeListQueryRequest
{
    [StringLength(200)]
    public string? Search { get; init; }

    public Guid? RecipeCategoryId { get; init; }

    public bool FavoritesOnly { get; init; }

    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 200)]
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
    [Required]
    [StringLength(200)]
    public string Title { get; init; } = string.Empty;

    public Guid? RecipeCategoryId { get; init; }

    [StringLength(1000)]
    public string? ImageUrl { get; init; }

    [StringLength(4000)]
    public string? Description { get; init; }

    [Range(0, int.MaxValue)]
    public int? PrepTimeMinutes { get; init; }

    [Range(0, int.MaxValue)]
    public int? WaitTimeMinutes { get; init; }

    public string? Instructions { get; init; }

    public bool IsManual { get; init; }

    public bool IsFavorite { get; init; }

    public IReadOnlyList<CreateRecipeIngredientRequest> Ingredients { get; init; } = [];
}

public sealed class UpdateRecipeRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; init; } = string.Empty;

    public Guid? RecipeCategoryId { get; init; }

    [StringLength(1000)]
    public string? ImageUrl { get; init; }

    [StringLength(4000)]
    public string? Description { get; init; }

    [Range(0, int.MaxValue)]
    public int? PrepTimeMinutes { get; init; }

    [Range(0, int.MaxValue)]
    public int? WaitTimeMinutes { get; init; }

    public string? Instructions { get; init; }

    public bool IsManual { get; init; }

    public bool IsFavorite { get; init; }
}
