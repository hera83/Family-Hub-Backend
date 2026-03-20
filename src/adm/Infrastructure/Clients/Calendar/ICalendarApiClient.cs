using FamilyHub.Adm.Models.Api.Calendar;

namespace FamilyHub.Adm.Infrastructure.Clients.Calendar;

public interface ICalendarApiClient
{
    Task<IReadOnlyList<FamilyMemberListItemDto>> GetFamilyMembersAsync(CancellationToken cancellationToken = default);
    Task<FamilyMemberDetailsDto> GetFamilyMemberByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FamilyMemberDetailsDto> CreateFamilyMemberAsync(CreateFamilyMemberRequest request, CancellationToken cancellationToken = default);
    Task<FamilyMemberDetailsDto> UpdateFamilyMemberAsync(Guid id, UpdateFamilyMemberRequest request, CancellationToken cancellationToken = default);
    Task DeleteFamilyMemberAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CalendarEventListItemDto>> GetCalendarEventsAsync(CalendarEventsQuery? query = null, CancellationToken cancellationToken = default);
    Task<CalendarEventDetailsDto> GetCalendarEventByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CalendarEventDetailsDto> CreateCalendarEventAsync(CreateCalendarEventRequest request, CancellationToken cancellationToken = default);
    Task<CalendarEventDetailsDto> UpdateCalendarEventAsync(Guid id, UpdateCalendarEventRequest request, CancellationToken cancellationToken = default);
    Task DeleteCalendarEventAsync(Guid id, CancellationToken cancellationToken = default);
}
