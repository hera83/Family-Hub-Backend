using FamilyHub.Api.Contracts.Sync;
using FamilyHub.Api.Entities;
using FamilyHub.Api.Features.Calendar;
using FamilyHub.Api.Features.Catalog;
using FamilyHub.Api.Features.Orders;
using FamilyHub.Api.Features.Recipes;
using FamilyHub.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Features.Sync;

/// <summary>
/// Aggregerer data på tværs af alle features til brug på touch-screen frontenden,
/// der typisk henter alt data ved opstart og synkroniserer periodisk.
/// </summary>
public sealed class SyncService(
    FamilyHubDbContext db,
    ISyncRequestValidator validator) : ISyncService
{
    public async Task<SyncManifestDto> GetManifestAsync(CancellationToken ct = default)
    {
        var datasets = new List<SyncManifestItemDto>
        {
            await BuildManifestItemAsync("familyMembers", db.FamilyMembers, ct),
            await BuildManifestItemAsync("calendarEvents", db.CalendarEvents, ct),
            await BuildManifestItemAsync("itemCategories", db.ItemCategories, ct),
            await BuildManifestItemAsync("products", db.Products, ct),
            await BuildManifestItemAsync("recipeCategories", db.RecipeCategories, ct),
            await BuildManifestItemAsync("recipes", db.Recipes, ct),
            await BuildManifestItemAsync("recipeIngredients", db.RecipeIngredients, ct),
            await BuildManifestItemAsync("orders", db.Orders, ct),
            await BuildManifestItemAsync("orderLines", db.OrderLines, ct)
        };

        return new SyncManifestDto(DateTime.UtcNow, datasets);
    }

    public async Task<FullSyncDto> GetFullSyncAsync(CancellationToken ct = default)
    {
        var familyMembers = (await db.FamilyMembers
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        var calendarEvents = (await db.CalendarEvents
            .AsNoTracking()
            .Include(x => x.FamilyMember)
            .OrderBy(x => x.EventDate)
            .ThenBy(x => x.StartTime)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        var itemCategories = (await db.ItemCategories
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

        var orders = (await db.Orders
            .AsNoTracking()
            .Include(o => o.Lines)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(ct))
            .Select(o => o.ToDetailsDto())
            .ToList();

        var orderLines = (await db.OrderLines
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ThenBy(x => x.OrderId)
            .ToListAsync(ct))
            .Select(x => x.ToLineDto())
            .ToList();

        return new FullSyncDto(
            DateTime.UtcNow,
            familyMembers,
            calendarEvents,
            itemCategories,
            products,
            recipeCategories,
            recipes,
            recipeIngredients,
            orders,
            orderLines);
    }

    public async Task<ChangesSinceDto> GetChangesSinceAsync(string? sinceUtc, CancellationToken ct = default)
    {
        var sinceUtcValue = validator.ValidateAndParseSinceUtc(sinceUtc);

        // V1: deletions spores ikke. /changes returnerer kun oprettede/opdaterede records.
        // Udvidelse i senere version kan ske via DeletedAtUtc eller separat change-log tabel.
        var familyMembers = (await db.FamilyMembers
            .AsNoTracking()
            .Where(x => (x.UpdatedAtUtc ?? x.CreatedAtUtc) > sinceUtcValue)
            .OrderBy(x => x.Name)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        var calendarEvents = (await db.CalendarEvents
            .AsNoTracking()
            .Include(x => x.FamilyMember)
            .Where(x => (x.UpdatedAtUtc ?? x.CreatedAtUtc) > sinceUtcValue)
            .OrderBy(x => x.EventDate)
            .ThenBy(x => x.StartTime)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        var itemCategories = (await db.ItemCategories
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

        var orders = (await db.Orders
            .AsNoTracking()
            .Include(o => o.Lines)
            .Where(o => (o.UpdatedAtUtc ?? o.CreatedAtUtc) > sinceUtcValue)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(ct))
            .Select(o => o.ToDetailsDto())
            .ToList();

        // V1: slettede ordrer/ordrelinjer spores ikke i /changes.
        // Endpointet returnerer kun oprettede/opdaterede records siden sinceUtc.
        var orderLines = (await db.OrderLines
            .AsNoTracking()
            .Where(x => (x.UpdatedAtUtc ?? x.CreatedAtUtc) > sinceUtcValue)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ThenBy(x => x.OrderId)
            .ToListAsync(ct))
            .Select(x => x.ToLineDto())
            .ToList();

        return new ChangesSinceDto(
            sinceUtcValue,
            DateTime.UtcNow,
            familyMembers,
            calendarEvents,
            itemCategories,
            products,
            recipeCategories,
            recipes,
            recipeIngredients,
            orders,
            orderLines);
    }

    private static async Task<SyncManifestItemDto> BuildManifestItemAsync<TEntity>(
        string datasetName,
        DbSet<TEntity> set,
        CancellationToken ct)
        where TEntity : BaseEntity
    {
        var totalCount = await set.CountAsync(ct);
        var lastModifiedUtc = totalCount == 0
            ? null
            : await set.MaxAsync(x => (DateTime?)(x.UpdatedAtUtc ?? x.CreatedAtUtc), ct);

        return new SyncManifestItemDto(datasetName, totalCount, lastModifiedUtc);
    }
}
