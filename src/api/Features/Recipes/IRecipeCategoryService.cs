using FamilyHub.Api.Contracts.Recipes;

namespace FamilyHub.Api.Features.Recipes;

public interface IRecipeCategoryService
{
    Task<IEnumerable<RecipeCategoryListItemDto>> GetAllAsync(CancellationToken ct = default);
    Task<RecipeCategoryDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<RecipeCategoryDetailsDto> CreateAsync(CreateRecipeCategoryRequest request, CancellationToken ct = default);
    Task<RecipeCategoryDetailsDto?> UpdateAsync(Guid id, UpdateRecipeCategoryRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
