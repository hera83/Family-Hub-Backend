using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Sync;

namespace FamilyHub.Adm.Infrastructure.Clients.Sync;

public sealed class SyncApiClient(HttpClient httpClient) : ApiClientBase(httpClient), ISyncApiClient
{
    public Task<SyncManifestDto> GetManifestAsync(CancellationToken cancellationToken = default)
        => GetAsync<SyncManifestDto>("api/v1/sync/manifest", cancellationToken);

    public Task<FullSyncDto> GetFullSyncAsync(CancellationToken cancellationToken = default)
        => GetAsync<FullSyncDto>("api/v1/sync/full", cancellationToken);
}
