using FamilyHub.Adm.Models.Api.Common;
using FamilyHub.Adm.Models.Api.Recipes;

namespace FamilyHub.Adm.Infrastructure.Clients.Recipes;

public interface IRecipesApiClient
{
    Task<IReadOnlyList<RecipeCategoryListItemDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<RecipeCategoryDetailsDto> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RecipeCategoryDetailsDto> CreateCategoryAsync(CreateRecipeCategoryRequest request, CancellationToken cancellationToken = default);
    Task<RecipeCategoryDetailsDto> UpdateCategoryAsync(Guid id, UpdateRecipeCategoryRequest request, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedListResponse<RecipeListItemDto>> GetRecipesAsync(RecipeListQueryRequest? query = null, CancellationToken cancellationToken = default);
    Task<RecipeDetailsDto> GetRecipeByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RecipeDetailsDto> CreateRecipeAsync(CreateRecipeRequest request, CancellationToken cancellationToken = default);
    Task<RecipeDetailsDto> UpdateRecipeAsync(Guid id, UpdateRecipeRequest request, CancellationToken cancellationToken = default);
    Task DeleteRecipeAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecipeIngredientDto>> GetRecipeIngredientsAsync(Guid recipeId, CancellationToken cancellationToken = default);
    Task<RecipeIngredientDto> AddIngredientAsync(Guid recipeId, CreateRecipeIngredientRequest request, CancellationToken cancellationToken = default);
    Task<RecipeIngredientDto> UpdateIngredientAsync(Guid recipeId, Guid ingredientId, UpdateRecipeIngredientRequest request, CancellationToken cancellationToken = default);
    Task DeleteIngredientAsync(Guid recipeId, Guid ingredientId, CancellationToken cancellationToken = default);

    Task<RecipeFullDto> GetRecipeFullAsync(Guid recipeId, CancellationToken cancellationToken = default);
}
