using FamilyHub.Api.Contracts.Catalog;
using FamilyHub.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Features.Catalog;

public sealed class CatalogSyncService(
    FamilyHubDbContext db,
    ICatalogSyncRequestValidator validator) : ICatalogSyncService
{
    public async Task<CatalogSyncDto> GetCurrentStateAsync(CancellationToken ct = default)
    {
        var categories = (await db.ItemCategories
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        var products = (await db.Products
            .AsNoTracking()
            .Include(x => x.ItemCategory)
            .OrderBy(x => x.Name)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        return new CatalogSyncDto(DateTime.UtcNow, categories, products);
    }

    public async Task<CatalogChangesSinceDto> GetChangesSinceAsync(string? sinceUtc, CancellationToken ct = default)
    {
        var sinceUtcValue = validator.ValidateAndParseSinceUtc(sinceUtc);

        var categories = (await db.ItemCategories
            .AsNoTracking()
            .Where(x => (x.UpdatedAtUtc ?? x.CreatedAtUtc) > sinceUtcValue)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        var products = (await db.Products
            .AsNoTracking()
            .Include(x => x.ItemCategory)
            .Where(x => (x.UpdatedAtUtc ?? x.CreatedAtUtc) > sinceUtcValue)
            .OrderBy(x => x.Name)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        return new CatalogChangesSinceDto(sinceUtcValue, DateTime.UtcNow, categories, products);
    }
}
