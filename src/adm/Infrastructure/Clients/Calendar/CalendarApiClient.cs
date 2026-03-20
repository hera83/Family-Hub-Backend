using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Calendar;

namespace FamilyHub.Adm.Infrastructure.Clients.Calendar;

public sealed class CalendarApiClient(HttpClient httpClient) : ApiClientBase(httpClient), ICalendarApiClient
{
    public Task<IReadOnlyList<FamilyMemberListItemDto>> GetFamilyMembersAsync(CancellationToken cancellationToken = default)
        => GetAsync<IReadOnlyList<FamilyMemberListItemDto>>("api/v1/calendar/members", cancellationToken);

    public Task<FamilyMemberDetailsDto> GetFamilyMemberByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => GetAsync<FamilyMemberDetailsDto>($"api/v1/calendar/members/{id}", cancellationToken);

    public Task<FamilyMemberDetailsDto> CreateFamilyMemberAsync(CreateFamilyMemberRequest request, CancellationToken cancellationToken = default)
        => PostAsync<CreateFamilyMemberRequest, FamilyMemberDetailsDto>("api/v1/calendar/members", request, cancellationToken);

    public Task<FamilyMemberDetailsDto> UpdateFamilyMemberAsync(Guid id, UpdateFamilyMemberRequest request, CancellationToken cancellationToken = default)
        => PutAsync<UpdateFamilyMemberRequest, FamilyMemberDetailsDto>($"api/v1/calendar/members/{id}", request, cancellationToken);

    public Task DeleteFamilyMemberAsync(Guid id, CancellationToken cancellationToken = default)
        => DeleteAsync($"api/v1/calendar/members/{id}", cancellationToken);

    public Task<IReadOnlyList<CalendarEventListItemDto>> GetCalendarEventsAsync(CalendarEventsQuery? query = null, CancellationToken cancellationToken = default)
    {
        var path = "api/v1/calendar/events";
        if (query is null)
        {
            return GetAsync<IReadOnlyList<CalendarEventListItemDto>>(path, cancellationToken);
        }

        var uri = WithQueryString(path, new Dictionary<string, string?>
        {
            ["fromDate"] = query.FromDate?.ToString("yyyy-MM-dd"),
            ["toDate"] = query.ToDate?.ToString("yyyy-MM-dd"),
            ["familyMemberId"] = query.FamilyMemberId?.ToString()
        });

        return GetAsync<IReadOnlyList<CalendarEventListItemDto>>(uri, cancellationToken);
    }

    public Task<CalendarEventDetailsDto> GetCalendarEventByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => GetAsync<CalendarEventDetailsDto>($"api/v1/calendar/events/{id}", cancellationToken);

    public Task<CalendarEventDetailsDto> CreateCalendarEventAsync(CreateCalendarEventRequest request, CancellationToken cancellationToken = default)
        => PostAsync<CreateCalendarEventRequest, CalendarEventDetailsDto>("api/v1/calendar/events", request, cancellationToken);

    public Task<CalendarEventDetailsDto> UpdateCalendarEventAsync(Guid id, UpdateCalendarEventRequest request, CancellationToken cancellationToken = default)
        => PutAsync<UpdateCalendarEventRequest, CalendarEventDetailsDto>($"api/v1/calendar/events/{id}", request, cancellationToken);

    public Task DeleteCalendarEventAsync(Guid id, CancellationToken cancellationToken = default)
        => DeleteAsync($"api/v1/calendar/events/{id}", cancellationToken);
}
