using FamilyHub.Api.Contracts.Calendar;

namespace FamilyHub.Api.Features.Calendar;

public interface ICalendarSyncService
{
    Task<CalendarSyncDto> GetCurrentStateAsync(CancellationToken ct = default);
    Task<CalendarChangesSinceDto> GetChangesSinceAsync(string? sinceUtc, CancellationToken ct = default);
}
