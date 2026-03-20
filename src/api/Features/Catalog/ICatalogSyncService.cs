using FamilyHub.Api.Contracts.Catalog;

namespace FamilyHub.Api.Features.Catalog;

public interface ICatalogSyncService
{
    Task<CatalogSyncDto> GetCurrentStateAsync(CancellationToken ct = default);
    Task<CatalogChangesSinceDto> GetChangesSinceAsync(string? sinceUtc, CancellationToken ct = default);
}
