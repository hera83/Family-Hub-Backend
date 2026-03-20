using FamilyHub.Adm.Models.Api.Sync;

namespace FamilyHub.Adm.Infrastructure.Clients.Sync;

public interface ISyncApiClient
{
    Task<SyncManifestDto> GetManifestAsync(CancellationToken cancellationToken = default);
    Task<FullSyncDto> GetFullSyncAsync(CancellationToken cancellationToken = default);
}
