using FamilyHub.Api.Contracts.Recipes;

namespace FamilyHub.Api.Features.Recipes;

public interface IRecipeSyncService
{
    Task<RecipeSyncDto> GetCurrentStateAsync(CancellationToken ct = default);
    Task<RecipeChangesSinceDto> GetChangesSinceAsync(string? sinceUtc, CancellationToken ct = default);
}
