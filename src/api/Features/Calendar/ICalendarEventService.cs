using FamilyHub.Api.Contracts.Calendar;

namespace FamilyHub.Api.Features.Calendar;

/// <summary>
/// Service-kontrakt for kalenderbegivenheder.
/// </summary>
public interface ICalendarEventService
{
    /// <summary>Hent begivenheder, filtreret på datointerval og/eller familiemedlem.</summary>
    Task<IEnumerable<CalendarEventListItemDto>> GetAllAsync(
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        Guid? familyMemberId = null,
        CancellationToken ct = default);

    Task<CalendarEventDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CalendarEventDetailsDto> CreateAsync(CreateCalendarEventRequest request, CancellationToken ct = default);
    Task<CalendarEventDetailsDto?> UpdateAsync(Guid id, UpdateCalendarEventRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
