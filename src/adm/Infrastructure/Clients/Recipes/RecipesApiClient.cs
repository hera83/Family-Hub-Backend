using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Common;
using FamilyHub.Adm.Models.Api.Recipes;

namespace FamilyHub.Adm.Infrastructure.Clients.Recipes;

public sealed class RecipesApiClient(HttpClient httpClient) : ApiClientBase(httpClient), IRecipesApiClient
{
    public Task<IReadOnlyList<RecipeCategoryListItemDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        => GetAsync<IReadOnlyList<RecipeCategoryListItemDto>>("api/v1/recipes/categories", cancellationToken);

    public Task<RecipeCategoryDetailsDto> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => GetAsync<RecipeCategoryDetailsDto>($"api/v1/recipes/categories/{id}", cancellationToken);

    public Task<RecipeCategoryDetailsDto> CreateCategoryAsync(CreateRecipeCategoryRequest request, CancellationToken cancellationToken = default)
        => PostAsync<CreateRecipeCategoryRequest, RecipeCategoryDetailsDto>("api/v1/recipes/categories", request, cancellationToken);

    public Task<RecipeCategoryDetailsDto> UpdateCategoryAsync(Guid id, UpdateRecipeCategoryRequest request, CancellationToken cancellationToken = default)
        => PutAsync<UpdateRecipeCategoryRequest, RecipeCategoryDetailsDto>($"api/v1/recipes/categories/{id}", request, cancellationToken);

    public Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
        => DeleteAsync($"api/v1/recipes/categories/{id}", cancellationToken);

    public Task<PagedListResponse<RecipeListItemDto>> GetRecipesAsync(RecipeListQueryRequest? query = null, CancellationToken cancellationToken = default)
    {
        var effectiveQuery = query ?? new RecipeListQueryRequest();

        var uri = WithQueryString("api/v1/recipes/items", new Dictionary<string, string?>
        {
            ["search"] = effectiveQuery.Search,
            ["recipeCategoryId"] = effectiveQuery.RecipeCategoryId?.ToString(),
            ["favoritesOnly"] = effectiveQuery.FavoritesOnly ? "true" : null,
            ["page"] = effectiveQuery.Page.ToString(),
            ["pageSize"] = effectiveQuery.PageSize.ToString()
        });

        return GetAsync<PagedListResponse<RecipeListItemDto>>(uri, cancellationToken);
    }

    public Task<RecipeDetailsDto> GetRecipeByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => GetAsync<RecipeDetailsDto>($"api/v1/recipes/items/{id}", cancellationToken);

    public Task<RecipeDetailsDto> CreateRecipeAsync(CreateRecipeRequest request, CancellationToken cancellationToken = default)
        => PostAsync<CreateRecipeRequest, RecipeDetailsDto>("api/v1/recipes/items", request, cancellationToken);

    public Task<RecipeDetailsDto> UpdateRecipeAsync(Guid id, UpdateRecipeRequest request, CancellationToken cancellationToken = default)
        => PutAsync<UpdateRecipeRequest, RecipeDetailsDto>($"api/v1/recipes/items/{id}", request, cancellationToken);

    public Task DeleteRecipeAsync(Guid id, CancellationToken cancellationToken = default)
        => DeleteAsync($"api/v1/recipes/items/{id}", cancellationToken);

    public Task<IReadOnlyList<RecipeIngredientDto>> GetRecipeIngredientsAsync(Guid recipeId, CancellationToken cancellationToken = default)
        => GetAsync<IReadOnlyList<RecipeIngredientDto>>($"api/v1/recipes/items/{recipeId}/ingredients", cancellationToken);

    public Task<RecipeIngredientDto> AddIngredientAsync(Guid recipeId, CreateRecipeIngredientRequest request, CancellationToken cancellationToken = default)
        => PostAsync<CreateRecipeIngredientRequest, RecipeIngredientDto>($"api/v1/recipes/items/{recipeId}/ingredients", request, cancellationToken);

    public Task<RecipeIngredientDto> UpdateIngredientAsync(Guid recipeId, Guid ingredientId, UpdateRecipeIngredientRequest request, CancellationToken cancellationToken = default)
        => PutAsync<UpdateRecipeIngredientRequest, RecipeIngredientDto>($"api/v1/recipes/items/{recipeId}/ingredients/{ingredientId}", request, cancellationToken);

    public Task DeleteIngredientAsync(Guid recipeId, Guid ingredientId, CancellationToken cancellationToken = default)
        => DeleteAsync($"api/v1/recipes/items/{recipeId}/ingredients/{ingredientId}", cancellationToken);

    public Task<RecipeFullDto> GetRecipeFullAsync(Guid recipeId, CancellationToken cancellationToken = default)
        => GetAsync<RecipeFullDto>($"api/v1/recipes/items/{recipeId}/full", cancellationToken);
}
