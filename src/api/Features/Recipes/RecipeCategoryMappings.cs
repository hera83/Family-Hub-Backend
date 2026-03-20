using FamilyHub.Api.Contracts.Recipes;
using FamilyHub.Api.Entities.Recipes;

namespace FamilyHub.Api.Features.Recipes;

internal static class RecipeCategoryMappings
{
    internal static RecipeCategoryListItemDto ToListItemDto(this RecipeCategory category) => new(
        category.Id,
        category.Name,
        category.SortOrder);

    internal static RecipeCategoryDetailsDto ToDetailsDto(this RecipeCategory category) => new(
        category.Id,
        category.Name,
        category.SortOrder,
        category.CreatedAtUtc,
        category.UpdatedAtUtc);

    internal static RecipeCategory ToEntity(this CreateRecipeCategoryRequest request) => new()
    {
        Name = request.Name,
        SortOrder = request.SortOrder
    };

    internal static void Apply(this RecipeCategory category, UpdateRecipeCategoryRequest request)
    {
        category.Name = request.Name;
        category.SortOrder = request.SortOrder;
    }
}