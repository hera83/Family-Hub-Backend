namespace FamilyHub.Api.Contracts.Recipes;

public sealed record RecipeSyncDto(
    DateTime GeneratedAtUtc,
    IReadOnlyList<RecipeCategoryDetailsDto> RecipeCategories,
    IReadOnlyList<RecipeDetailsDto> Recipes,
    IReadOnlyList<RecipeIngredientDto> RecipeIngredients
);

public sealed record RecipeChangesSinceDto(
    DateTime SinceUtc,
    DateTime GeneratedAtUtc,
    IReadOnlyList<RecipeCategoryDetailsDto> RecipeCategories,
    IReadOnlyList<RecipeDetailsDto> Recipes,
    IReadOnlyList<RecipeIngredientDto> RecipeIngredients
);
