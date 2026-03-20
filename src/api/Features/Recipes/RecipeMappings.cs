using FamilyHub.Api.Contracts.Recipes;
using FamilyHub.Api.Entities.Recipes;

namespace FamilyHub.Api.Features.Recipes;

internal static class RecipeMappings
{
    internal static RecipeListItemDto ToListItemDto(this Recipe recipe) => new(
        recipe.Id,
        recipe.Title,
        recipe.RecipeCategoryId,
        recipe.RecipeCategory?.Name,
        recipe.ImageUrl,
        recipe.PrepTimeMinutes,
        recipe.WaitTimeMinutes,
        recipe.IsFavorite);

    internal static RecipeDetailsDto ToDetailsDto(this Recipe recipe) => new(
        recipe.Id,
        recipe.Title,
        recipe.RecipeCategoryId,
        recipe.RecipeCategory?.Name,
        recipe.ImageUrl,
        recipe.Description,
        recipe.PrepTimeMinutes,
        recipe.WaitTimeMinutes,
        recipe.Instructions,
        recipe.IsManual,
        recipe.IsFavorite,
        recipe.CreatedAtUtc,
        recipe.UpdatedAtUtc);

    internal static RecipeFullDto ToFullDto(this Recipe recipe) => new(
        recipe.Id,
        recipe.Title,
        recipe.RecipeCategoryId,
        recipe.RecipeCategory?.Name,
        recipe.ImageUrl,
        recipe.Description,
        recipe.PrepTimeMinutes,
        recipe.WaitTimeMinutes,
        recipe.Instructions,
        recipe.IsManual,
        recipe.IsFavorite,
        recipe.Ingredients
            .OrderBy(i => i.SortOrder)
            .Select(i => i.ToDto())
            .ToList(),
        recipe.CreatedAtUtc,
        recipe.UpdatedAtUtc);

    internal static RecipeIngredientDto ToDto(this RecipeIngredient ingredient) => new(
        ingredient.Id,
        ingredient.RecipeId,
        ingredient.ProductId,
        ingredient.Product?.Name,
        ingredient.Product?.ItemCategory?.Name,
        ingredient.Name,
        ingredient.Quantity,
        ingredient.Unit,
        ingredient.IsStaple,
        ingredient.SortOrder,
        ingredient.CreatedAtUtc,
        ingredient.UpdatedAtUtc);

    internal static Recipe ToEntity(this CreateRecipeRequest request) => new()
    {
        Title = request.Title,
        RecipeCategoryId = request.RecipeCategoryId,
        ImageUrl = request.ImageUrl,
        Description = request.Description,
        PrepTimeMinutes = request.PrepTimeMinutes,
        WaitTimeMinutes = request.WaitTimeMinutes,
        Instructions = request.Instructions,
        IsManual = request.IsManual,
        IsFavorite = request.IsFavorite,
        Ingredients = request.Ingredients
            .Select((ingredient, index) => ingredient.ToEntity(Guid.Empty, index))
            .ToList()
    };

    internal static RecipeIngredient ToEntity(this CreateRecipeIngredientRequest request, Guid recipeId)
        => request.ToEntity(recipeId, null);

    internal static void Apply(this Recipe recipe, UpdateRecipeRequest request)
    {
        recipe.Title = request.Title;
        recipe.RecipeCategoryId = request.RecipeCategoryId;
        recipe.ImageUrl = request.ImageUrl;
        recipe.Description = request.Description;
        recipe.PrepTimeMinutes = request.PrepTimeMinutes;
        recipe.WaitTimeMinutes = request.WaitTimeMinutes;
        recipe.Instructions = request.Instructions;
        recipe.IsManual = request.IsManual;
        recipe.IsFavorite = request.IsFavorite;
    }

    internal static void Apply(this RecipeIngredient ingredient, UpdateRecipeIngredientRequest request)
    {
        ingredient.ProductId = request.ProductId;
        ingredient.Name = request.Name;
        ingredient.Quantity = request.Quantity;
        ingredient.Unit = request.Unit;
        ingredient.IsStaple = request.IsStaple;
        ingredient.SortOrder = request.SortOrder;
    }

    private static RecipeIngredient ToEntity(
        this CreateRecipeIngredientRequest request,
        Guid recipeId,
        int? fallbackSortOrder) => new()
    {
        RecipeId = recipeId,
        ProductId = request.ProductId,
        Name = request.Name,
        Quantity = request.Quantity,
        Unit = request.Unit,
        IsStaple = request.IsStaple,
        SortOrder = request.SortOrder == 0 && fallbackSortOrder.HasValue ? fallbackSortOrder.Value : request.SortOrder
    };
}