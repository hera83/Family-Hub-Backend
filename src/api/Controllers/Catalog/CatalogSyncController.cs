using FamilyHub.Api.Features.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHub.Api.Controllers.Catalog;

/// <summary>
/// Sync-endpoints for Catalog-featuret.
/// </summary>
[Route("api/v1/catalog/sync")]
public sealed class CatalogSyncController(ICatalogSyncService service) : ApiControllerBase
{
    /// <summary>
    /// Returnerer den samlede catalog-payload til initial indlæsning.
    /// Svaret indeholder itemCategories, products og generatedAtUtc.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken ct)
        => OkResponse(await service.GetCurrentStateAsync(ct));

    /// <summary>
    /// Returnerer kun kategorier og produkter ændret efter et givent UTC-tidspunkt.
    /// Eksempel: /api/v1/catalog/sync/changes?sinceUtc=2026-03-19T08:30:00Z
    /// </summary>
    [HttpGet("changes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetChanges([FromQuery] string? sinceUtc, CancellationToken ct)
        => OkResponse(await service.GetChangesSinceAsync(sinceUtc, ct));
}
