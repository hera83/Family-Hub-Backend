using FamilyHub.Api.Features.Sync;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHub.Api.Controllers.Sync;

/// <summary>
/// Sync-endpoints til touch-screen frontends der skal hente/opfriske al data.
/// </summary>
[Route("api/v1/sync")]
public sealed class SyncController(ISyncService syncService) : ApiControllerBase
{
    /// <summary>
    /// Returnerer et manifest med tæller per domæne og tidspunkt for seneste ændring.
    /// Frontenden bruger dette til at vurdere om en fuld eller delta sync er nødvendig.
    /// </summary>
    [HttpGet("manifest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetManifest(CancellationToken ct)
        => OkResponse(await syncService.GetManifestAsync(ct));

    /// <summary>
    /// Returnerer komplet datapakke på tværs af alle domæner.
    /// Bruges til initial load eller ved tvunget nulstilling af frontend-cache.
    /// </summary>
    [HttpGet("full")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFullSync(CancellationToken ct)
        => OkResponse(await syncService.GetFullSyncAsync(ct));

    /// <summary>
    /// Returnerer kun records der er ændret siden et givent tidspunkt (ISO 8601 UTC).
    /// Bruges til periodisk delta sync fra frontenden.
    /// Eksempel: <c>/api/v1/sync/changes?sinceUtc=2025-01-01T00:00:00Z</c>
    /// </summary>
    [HttpGet("changes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetChangesSince([FromQuery] string? sinceUtc, CancellationToken ct)
        => OkResponse(await syncService.GetChangesSinceAsync(sinceUtc, ct));
}
