using FamilyHub.Api.Features.Orders;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHub.Api.Controllers.Orders;

/// <summary>
/// Sync-endpoints for Orders-featuret.
/// </summary>
[Route("api/v1/orders/sync")]
public sealed class OrderSyncController(IOrderSyncService service) : ApiControllerBase
{
    /// <summary>
    /// Returnerer alle ordrer og ordrelinjer til initial indlæsning.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken ct)
        => OkResponse(await service.GetCurrentStateAsync(ct));

    /// <summary>
    /// Returnerer ordrer og ordrelinjer ændret efter et givent UTC-tidspunkt.
    /// Eksempel: /api/v1/orders/sync/changes?sinceUtc=2026-03-19T08:30:00Z
    /// </summary>
    [HttpGet("changes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetChanges([FromQuery] string? sinceUtc, CancellationToken ct)
        => OkResponse(await service.GetChangesSinceAsync(sinceUtc, ct));
}
