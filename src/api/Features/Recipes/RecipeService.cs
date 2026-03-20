using FamilyHub.Api.Contracts.Recipes;
using FamilyHub.Api.Contracts.Common;
using FamilyHub.Api.Entities.Recipes;
using FamilyHub.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Features.Recipes;

public sealed class RecipeService(
    FamilyHubDbContext db,
    IRecipeRequestValidator validator,
    IRecipeIngredientRequestValidator ingredientValidator) : IRecipeService
{
    public async Task<PagedListResponse<RecipeListItemDto>> GetAllAsync(RecipeListQueryRequest query, CancellationToken ct = default)
    {
        validator.Validate(query);

        var recipeQuery = db.Recipes
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            recipeQuery = recipeQuery.Where(r => EF.Functions.Like(r.Title, $"%{search}%"));
        }

        if (query.RecipeCategoryId.HasValue)
            recipeQuery = recipeQuery.Where(r => r.RecipeCategoryId == query.RecipeCategoryId.Value);

        if (query.FavoritesOnly)
            recipeQuery = recipeQuery.Where(r => r.IsFavorite);

        var totalCount = await recipeQuery.CountAsync(ct);

        var recipes = await recipeQuery
            .Include(r => r.RecipeCategory)
            .OrderBy(r => r.Title)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return new PagedListResponse<RecipeListItemDto>
        {
            Items = recipes.Select(r => r.ToListItemDto()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<RecipeDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var recipe = await db.Recipes
            .AsNoTracking()
            .Include(r => r.RecipeCategory)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        return recipe?.ToDetailsDto();
    }

    public async Task<RecipeFullDto?> GetFullByIdAsync(Guid id, CancellationToken ct = default)
    {
        var recipe = await db.Recipes
            .AsNoTracking()
            .Include(r => r.RecipeCategory)
            .Include(r => r.Ingredients.OrderBy(i => i.SortOrder))
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p!.ItemCategory)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        return recipe?.ToFullDto();
    }

    public async Task<RecipeFullDto> CreateAsync(CreateRecipeRequest request, CancellationToken ct = default)
    {
        validator.Validate(request);

        if (request.RecipeCategoryId.HasValue)
        {
            var categoryExists = await db.RecipeCategories
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.RecipeCategoryId.Value, ct);

            if (!categoryExists)
                throw new ArgumentException("Det angivne recipeCategoryId findes ikke.");
        }

        if (request.Ingredients.Count > 0)
        {
            foreach (var ingredient in request.Ingredients)
                ingredientValidator.Validate(ingredient);
        }

        var recipe = request.ToEntity();

        db.Recipes.Add(recipe);
        await db.SaveChangesAsync(ct);

        return (await GetFullByIdAsync(recipe.Id, ct))!;
    }

    public async Task<RecipeDetailsDto?> UpdateAsync(Guid id, UpdateRecipeRequest request, CancellationToken ct = default)
    {
        validator.Validate(request);

        if (request.RecipeCategoryId.HasValue)
        {
            var categoryExists = await db.RecipeCategories
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.RecipeCategoryId.Value, ct);

            if (!categoryExists)
                throw new ArgumentException("Det angivne recipeCategoryId findes ikke.");
        }

        var recipe = await db.Recipes.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (recipe is null) return null;

        recipe.Apply(request);

        await db.SaveChangesAsync(ct);

        if (recipe.RecipeCategoryId is null)
        {
            recipe.RecipeCategory = null;
        }
        else
        {
            db.Entry(recipe).Reference(r => r.RecipeCategory).IsLoaded = false;
            await db.Entry(recipe).Reference(r => r.RecipeCategory).LoadAsync(ct);
        }

        return recipe.ToDetailsDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var recipe = await db.Recipes.FindAsync([id], ct);
        if (recipe is null) return false;
        db.Recipes.Remove(recipe);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<RecipeIngredientDto>?> GetIngredientsAsync(Guid recipeId, CancellationToken ct = default)
    {
        var recipeExists = await db.Recipes.AsNoTracking().AnyAsync(r => r.Id == recipeId, ct);
        if (!recipeExists) return null;

        var ingredients = await db.RecipeIngredients
            .AsNoTracking()
            .Include(i => i.Product)
            .ThenInclude(p => p!.ItemCategory)
            .Where(i => i.RecipeId == recipeId)
            .OrderBy(i => i.SortOrder)
            .ThenBy(i => i.Name ?? i.Product!.Name)
            .ToListAsync(ct);

        return ingredients.Select(i => i.ToDto()).ToList();
    }

    public async Task<RecipeIngredientDto> AddIngredientAsync(
        Guid recipeId, CreateRecipeIngredientRequest request, CancellationToken ct = default)
    {
        ingredientValidator.Validate(request);

        var recipeExists = await db.Recipes.AsNoTracking().AnyAsync(r => r.Id == recipeId, ct);
        if (!recipeExists)
            throw new KeyNotFoundException("Opskriften blev ikke fundet.");

        if (request.ProductId.HasValue)
        {
            var productExists = await db.Products.AsNoTracking().AnyAsync(p => p.Id == request.ProductId.Value, ct);
            if (!productExists)
                throw new ArgumentException("Det angivne productId findes ikke.");
        }

        var ingredient = request.ToEntity(recipeId);

        db.RecipeIngredients.Add(ingredient);
        await db.SaveChangesAsync(ct);

        var persistedIngredient = await db.RecipeIngredients
            .AsNoTracking()
            .Include(i => i.Product)
            .ThenInclude(p => p!.ItemCategory)
            .FirstAsync(i => i.Id == ingredient.Id, ct);

        return persistedIngredient.ToDto();
    }

    public async Task<RecipeIngredientDto?> UpdateIngredientAsync(
        Guid recipeId, Guid ingredientId, UpdateRecipeIngredientRequest request, CancellationToken ct = default)
    {
        ingredientValidator.Validate(request);

        if (request.ProductId.HasValue)
        {
            var productExists = await db.Products.AsNoTracking().AnyAsync(p => p.Id == request.ProductId.Value, ct);
            if (!productExists)
                throw new ArgumentException("Det angivne productId findes ikke.");
        }

        var ingredient = await db.RecipeIngredients
            .FirstOrDefaultAsync(i => i.Id == ingredientId && i.RecipeId == recipeId, ct);
        if (ingredient is null) return null;

        ingredient.Apply(request);

        await db.SaveChangesAsync(ct);

        var persistedIngredient = await db.RecipeIngredients
            .AsNoTracking()
            .Include(i => i.Product)
            .ThenInclude(p => p!.ItemCategory)
            .FirstAsync(i => i.Id == ingredient.Id, ct);

        return persistedIngredient.ToDto();
    }

    public async Task<bool> DeleteIngredientAsync(Guid recipeId, Guid ingredientId, CancellationToken ct = default)
    {
        var ingredient = await db.RecipeIngredients
            .FirstOrDefaultAsync(i => i.Id == ingredientId && i.RecipeId == recipeId, ct);
        if (ingredient is null) return false;
        db.RecipeIngredients.Remove(ingredient);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
