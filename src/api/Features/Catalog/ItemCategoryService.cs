using FamilyHub.Api.Contracts.Catalog;
using FamilyHub.Api.Entities.Catalog;
using FamilyHub.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Features.Catalog;

public sealed class ItemCategoryService(
    FamilyHubDbContext db,
    IItemCategoryRequestValidator validator) : IItemCategoryService
{
    public async Task<IEnumerable<ItemCategoryListItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await db.ItemCategories
            .AsNoTracking()
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Name)
            .ToListAsync(ct);

        return categories.Select(c => c.ToListItemDto()).ToList();
    }

    public async Task<ItemCategoryDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cat = await db.ItemCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
        return cat?.ToDetailsDto();
    }

    public async Task<ItemCategoryDetailsDto> CreateAsync(CreateItemCategoryRequest request, CancellationToken ct = default)
    {
        validator.Validate(request);

        var cat = request.ToEntity();
        db.ItemCategories.Add(cat);
        await db.SaveChangesAsync(ct);
        return cat.ToDetailsDto();
    }

    public async Task<ItemCategoryDetailsDto?> UpdateAsync(Guid id, UpdateItemCategoryRequest request, CancellationToken ct = default)
    {
        validator.Validate(request);

        var cat = await db.ItemCategories.FindAsync([id], ct);
        if (cat is null) return null;
        cat.Apply(request);
        await db.SaveChangesAsync(ct);
        return cat.ToDetailsDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var cat = await db.ItemCategories.FindAsync([id], ct);
        if (cat is null) return false;

        var hasProducts = await db.Products
            .AsNoTracking()
            .AnyAsync(x => x.ItemCategoryId == id, ct);

        // Sikreste løsning: afvis sletning, så produkter ikke utilsigtet mister deres kategori.
        if (hasProducts)
            throw new ArgumentException("Kategorien kan ikke slettes, fordi der stadig findes produkter i den.");

        db.ItemCategories.Remove(cat);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
