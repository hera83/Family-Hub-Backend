using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyHub.Adm.Infrastructure.Clients.Calendar;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Calendar;

namespace FamilyHub.Adm.Pages.Calendar;

public class EventsModel(ICalendarApiClient calendarApiClient) : PageModel
{
    private static readonly Dictionary<int, string> WeekDays = new()
    {
        [1] = "Man",
        [2] = "Tir",
        [3] = "Ons",
        [4] = "Tor",
        [5] = "Fre",
        [6] = "Loer",
        [0] = "Soen"
    };

    private readonly ICalendarApiClient _calendarApiClient = calendarApiClient;

    public IReadOnlyList<CalendarEventListItemViewModel> Items { get; private set; } = [];

    public string? LoadErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var events = await _calendarApiClient.GetCalendarEventsAsync(null, cancellationToken);
            Items = events
                .OrderBy(x => x.EventDate)
                .ThenBy(x => x.StartTime)
                .Select(x => new CalendarEventListItemViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    EventDate = x.EventDate,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    FamilyMemberName = x.FamilyMemberName,
                    FamilyMemberColor = x.FamilyMemberColor,
                    RecurrenceType = x.RecurrenceType,
                    RecurrenceDaysDisplay = ToDayNames(x.RecurrenceDays)
                })
                .ToArray();
        }
        catch (ApiClientException ex)
        {
            LoadErrorMessage = ex.UserMessage;
        }
    }

    public string FormatTimeRange(TimeOnly? start, TimeOnly? end)
    {
        if (start is null && end is null)
        {
            return "-";
        }

        var startText = start?.ToString("HH:mm") ?? "-";
        var endText = end?.ToString("HH:mm") ?? "-";
        return $"{startText} - {endText}";
    }

    public string FormatRecurrence(CalendarEventListItemViewModel item)
    {
        if (string.IsNullOrWhiteSpace(item.RecurrenceType))
        {
            return "Ingen";
        }

        if (string.IsNullOrWhiteSpace(item.RecurrenceDaysDisplay))
        {
            return item.RecurrenceType;
        }

        return $"{item.RecurrenceType} ({item.RecurrenceDaysDisplay})";
    }

    private static string? ToDayNames(int[]? recurrenceDays)
    {
        if (recurrenceDays is null || recurrenceDays.Length == 0)
        {
            return null;
        }

        return string.Join(", ",
            recurrenceDays
                .Distinct()
                .OrderBy(x => x)
                .Select(day => WeekDays.TryGetValue(day, out var label) ? label : day.ToString()));
    }
}
