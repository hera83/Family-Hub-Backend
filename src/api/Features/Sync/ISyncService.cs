using FamilyHub.Api.Contracts.Sync;

namespace FamilyHub.Api.Features.Sync;

public interface ISyncService
{
    /// <summary>Returnerer manifest med tæller og seneste ændring.</summary>
    Task<SyncManifestDto> GetManifestAsync(CancellationToken ct = default);

    /// <summary>Returnerer komplet datapakke til initial sync.</summary>
    Task<FullSyncDto> GetFullSyncAsync(CancellationToken ct = default);

    /// <summary>Returnerer alle records opdateret siden <paramref name="sinceUtc"/>.</summary>
    Task<ChangesSinceDto> GetChangesSinceAsync(string? sinceUtc, CancellationToken ct = default);
}
