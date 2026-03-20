using FamilyHub.Api.Contracts.Recipes;
using FamilyHub.Api.Contracts.Common;

namespace FamilyHub.Api.Features.Recipes;

public interface IRecipeService
{
    Task<PagedListResponse<RecipeListItemDto>> GetAllAsync(RecipeListQueryRequest query, CancellationToken ct = default);
    Task<RecipeDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<RecipeFullDto?> GetFullByIdAsync(Guid id, CancellationToken ct = default);
    Task<RecipeFullDto> CreateAsync(CreateRecipeRequest request, CancellationToken ct = default);
    Task<RecipeDetailsDto?> UpdateAsync(Guid id, UpdateRecipeRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<RecipeIngredientDto>?> GetIngredientsAsync(Guid recipeId, CancellationToken ct = default);
    Task<RecipeIngredientDto> AddIngredientAsync(Guid recipeId, CreateRecipeIngredientRequest request, CancellationToken ct = default);
    Task<RecipeIngredientDto?> UpdateIngredientAsync(Guid recipeId, Guid ingredientId, UpdateRecipeIngredientRequest request, CancellationToken ct = default);
    Task<bool> DeleteIngredientAsync(Guid recipeId, Guid ingredientId, CancellationToken ct = default);
}
