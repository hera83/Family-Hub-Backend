using FamilyHub.Api.Contracts.Recipes;
using FamilyHub.Api.Entities.Recipes;
using FamilyHub.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Features.Recipes;

public sealed class RecipeCategoryService(
    FamilyHubDbContext db,
    IRecipeCategoryRequestValidator validator) : IRecipeCategoryService
{
    public async Task<IEnumerable<RecipeCategoryListItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await db.RecipeCategories
            .AsNoTracking()
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Name)
            .ToListAsync(ct);

        return categories.Select(c => c.ToListItemDto()).ToList();
    }

    public async Task<RecipeCategoryDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var c = await db.RecipeCategories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return c?.ToDetailsDto();
    }

    public async Task<RecipeCategoryDetailsDto> CreateAsync(CreateRecipeCategoryRequest request, CancellationToken ct = default)
    {
        validator.Validate(request);

        var cat = request.ToEntity();
        db.RecipeCategories.Add(cat);
        await db.SaveChangesAsync(ct);
        return cat.ToDetailsDto();
    }

    public async Task<RecipeCategoryDetailsDto?> UpdateAsync(Guid id, UpdateRecipeCategoryRequest request, CancellationToken ct = default)
    {
        validator.Validate(request);

        var cat = await db.RecipeCategories.FindAsync([id], ct);
        if (cat is null) return null;
        cat.Apply(request);
        await db.SaveChangesAsync(ct);
        return cat.ToDetailsDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var cat = await db.RecipeCategories.FindAsync([id], ct);
        if (cat is null) return false;

        var hasRecipes = await db.Recipes
            .AsNoTracking()
            .AnyAsync(x => x.RecipeCategoryId == id, ct);

        // Sikreste løsning: afvis sletning, så opskrifter ikke utilsigtet mister deres kategori.
        if (hasRecipes)
            throw new ArgumentException("Opskriftskategorien kan ikke slettes, fordi der stadig findes opskrifter i den.");

        db.RecipeCategories.Remove(cat);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
