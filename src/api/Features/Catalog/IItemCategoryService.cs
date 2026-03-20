using FamilyHub.Api.Contracts.Catalog;

namespace FamilyHub.Api.Features.Catalog;

public interface IItemCategoryService
{
    Task<IEnumerable<ItemCategoryListItemDto>> GetAllAsync(CancellationToken ct = default);
    Task<ItemCategoryDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ItemCategoryDetailsDto> CreateAsync(CreateItemCategoryRequest request, CancellationToken ct = default);
    Task<ItemCategoryDetailsDto?> UpdateAsync(Guid id, UpdateItemCategoryRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
