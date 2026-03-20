using FamilyHub.Api.Contracts.Calendar;
using FamilyHub.Api.Features.Calendar;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHub.Api.Controllers.Calendar;

/// <summary>
/// CRUD-endpoints for kalenderbegivenheder med optional filtrering.
/// </summary>
[Route("api/v1/calendar/events")]
public sealed class CalendarEventsController(ICalendarEventService service) : ApiControllerBase
{
    /// <summary>
    /// Hent kalenderbegivenheder sorteret på dato og starttid.
    /// Kan filtreres med <paramref name="fromDate"/>, <paramref name="toDate"/> og <paramref name="familyMemberId"/>.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] Guid? familyMemberId,
        CancellationToken ct)
        => OkResponse(await service.GetAllAsync(fromDate, toDate, familyMemberId, ct));

    /// <summary>Hent én begivenhed via ID.</summary>
    [HttpGet("{id:guid}", Name = nameof(GetCalendarEventById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCalendarEventById(Guid id, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>
    /// Opret ny kalenderbegivenhed.
    /// Eksempel request: { "title": "Forældremøde", "eventDate": "2026-03-20", "startTime": "17:00:00", "endTime": "18:00:00", "familyMemberId": null, "recurrenceType": "Weekly", "recurrenceDays": [2,4] }
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCalendarEventRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedResponse(nameof(GetCalendarEventById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Opdater en eksisterende kalenderbegivenhed.
    /// Eksempel request: { "title": "Forældremøde", "eventDate": "2026-03-20", "startTime": "17:30:00", "endTime": "18:30:00", "familyMemberId": null, "recurrenceType": null, "recurrenceDays": null }
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCalendarEventRequest request, CancellationToken ct)
    {
        var result = await service.UpdateAsync(id, request, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>Slet en kalenderbegivenhed.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFoundResponse();
    }
}
