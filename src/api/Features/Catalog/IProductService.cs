using FamilyHub.Api.Contracts.Catalog;
using FamilyHub.Api.Contracts.Common;

namespace FamilyHub.Api.Features.Catalog;

public interface IProductService
{
    Task<PagedListResponse<ProductListItemDto>> GetAllAsync(ProductListQueryRequest query, CancellationToken ct = default);
    Task<ProductDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductDetailsDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<ProductDetailsDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
