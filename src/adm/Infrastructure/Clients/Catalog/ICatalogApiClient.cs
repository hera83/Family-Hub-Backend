using FamilyHub.Adm.Models.Api.Catalog;
using FamilyHub.Adm.Models.Api.Common;

namespace FamilyHub.Adm.Infrastructure.Clients.Catalog;

public interface ICatalogApiClient
{
    Task<IReadOnlyList<ItemCategoryListItemDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<ItemCategoryDetailsDto> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ItemCategoryDetailsDto> CreateCategoryAsync(CreateItemCategoryRequest request, CancellationToken cancellationToken = default);
    Task<ItemCategoryDetailsDto> UpdateCategoryAsync(Guid id, UpdateItemCategoryRequest request, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedListResponse<ProductListItemDto>> GetProductsAsync(ProductListQueryRequest? query = null, CancellationToken cancellationToken = default);
    Task<ProductDetailsDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductDetailsDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<ProductDetailsDto> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
}
