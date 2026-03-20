using FamilyHub.Api.Contracts.Recipes;
using FamilyHub.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Features.Recipes;

public sealed class RecipeSyncService(
    FamilyHubDbContext db,
    IRecipeSyncRequestValidator validator) : IRecipeSyncService
{
    public async Task<RecipeSyncDto> GetCurrentStateAsync(CancellationToken ct = default)
    {
        var recipeCategories = (await db.RecipeCategories
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        var recipes = (await db.Recipes
            .AsNoTracking()
            .Include(x => x.RecipeCategory)
            .OrderBy(x => x.Title)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        var recipeIngredients = (await db.RecipeIngredients
            .AsNoTracking()
            .Include(x => x.Product)
            .ThenInclude(p => p!.ItemCategory)
            .OrderBy(x => x.RecipeId)
            .ThenBy(x => x.SortOrder)
            .ToListAsync(ct))
            .Select(x => x.ToDto())
            .ToList();

        return new RecipeSyncDto(DateTime.UtcNow, recipeCategories, recipes, recipeIngredients);
    }

    public async Task<RecipeChangesSinceDto> GetChangesSinceAsync(string? sinceUtc, CancellationToken ct = default)
    {
        var sinceUtcValue = validator.ValidateAndParseSinceUtc(sinceUtc);

        var recipeCategories = (await db.RecipeCategories
            .AsNoTracking()
            .Where(x => (x.UpdatedAtUtc ?? x.CreatedAtUtc) > sinceUtcValue)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        var recipes = (await db.Recipes
            .AsNoTracking()
            .Include(x => x.RecipeCategory)
            .Where(x => (x.UpdatedAtUtc ?? x.CreatedAtUtc) > sinceUtcValue)
            .OrderBy(x => x.Title)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        var recipeIngredients = (await db.RecipeIngredients
            .AsNoTracking()
            .Include(x => x.Product)
            .ThenInclude(p => p!.ItemCategory)
            .Where(x => (x.UpdatedAtUtc ?? x.CreatedAtUtc) > sinceUtcValue)
            .OrderBy(x => x.RecipeId)
            .ThenBy(x => x.SortOrder)
            .ToListAsync(ct))
            .Select(x => x.ToDto())
            .ToList();

        return new RecipeChangesSinceDto(sinceUtcValue, DateTime.UtcNow, recipeCategories, recipes, recipeIngredients);
    }
}
