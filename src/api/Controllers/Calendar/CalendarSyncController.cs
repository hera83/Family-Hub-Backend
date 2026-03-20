using FamilyHub.Api.Features.Calendar;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHub.Api.Controllers.Calendar;

/// <summary>
/// Sync-endpoints for Calendar-featuret.
/// </summary>
[Route("api/v1/calendar/sync")]
public sealed class CalendarSyncController(ICalendarSyncService service) : ApiControllerBase
{
    /// <summary>
    /// Returnerer den samlede Calendar-payload til initial indlæsning.
    /// Svaret indeholder familyMembers, calendarEvents og generatedAtUtc.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken ct)
        => OkResponse(await service.GetCurrentStateAsync(ct));

    /// <summary>
    /// Returnerer kun members og events ændret efter et givent UTC-tidspunkt.
    /// Eksempel: /api/v1/calendar/sync/changes?sinceUtc=2026-03-19T08:30:00Z
    /// </summary>
    [HttpGet("changes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetChanges([FromQuery] string? sinceUtc, CancellationToken ct)
        => OkResponse(await service.GetChangesSinceAsync(sinceUtc, ct));
}
