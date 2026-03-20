using FamilyHub.Api.Contracts.Catalog;
using FamilyHub.Api.Contracts.Common;
using FamilyHub.Api.Entities.Catalog;
using FamilyHub.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Features.Catalog;

public sealed class ProductService(
    FamilyHubDbContext db,
    IProductRequestValidator validator) : IProductService
{
    public async Task<PagedListResponse<ProductListItemDto>> GetAllAsync(ProductListQueryRequest query, CancellationToken ct = default)
    {
        validator.Validate(query);

        var productQuery = db.Products
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            productQuery = productQuery.Where(p => EF.Functions.Like(p.Name, $"%{search}%"));
        }

        if (query.ItemCategoryId.HasValue)
            productQuery = productQuery.Where(p => p.ItemCategoryId == query.ItemCategoryId.Value);

        if (query.FavoritesOnly)
            productQuery = productQuery.Where(p => p.IsFavorite);

        if (query.StaplesOnly)
            productQuery = productQuery.Where(p => p.IsStaple);

        var totalCount = await productQuery.CountAsync(ct);

        var products = await productQuery
            .Include(p => p.ItemCategory)
            .OrderBy(p => p.Name)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return new PagedListResponse<ProductListItemDto>
        {
            Items = products.Select(p => p.ToListItemDto()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ProductDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var p = await db.Products.AsNoTracking()
            .Include(p => p.ItemCategory)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        return p?.ToDetailsDto();
    }

    public async Task<ProductDetailsDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        validator.Validate(request);

        if (request.ItemCategoryId.HasValue)
        {
            var categoryExists = await db.ItemCategories
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.ItemCategoryId.Value, ct);

            if (!categoryExists)
                throw new ArgumentException("Det angivne itemCategoryId findes ikke.");
        }

        var product = request.ToEntity();
        db.Products.Add(product);
        await db.SaveChangesAsync(ct);
        await db.Entry(product).Reference(p => p.ItemCategory).LoadAsync(ct);
        return product.ToDetailsDto();
    }

    public async Task<ProductDetailsDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        validator.Validate(request);

        if (request.ItemCategoryId.HasValue)
        {
            var categoryExists = await db.ItemCategories
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.ItemCategoryId.Value, ct);

            if (!categoryExists)
                throw new ArgumentException("Det angivne itemCategoryId findes ikke.");
        }

        var product = await db.Products
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        if (product is null) return null;

        product.Apply(request);

        await db.SaveChangesAsync(ct);

        if (product.ItemCategoryId is null)
        {
            product.ItemCategory = null;
        }
        else
        {
            db.Entry(product).Reference(p => p.ItemCategory).IsLoaded = false;
            await db.Entry(product).Reference(p => p.ItemCategory).LoadAsync(ct);
        }

        return product.ToDetailsDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await db.Products.FindAsync([id], ct);
        if (product is null) return false;
        db.Products.Remove(product);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
