using System.Text.Json;
using FamilyHub.Api.Contracts.Calendar;
using FamilyHub.Api.Entities.Calendar;

namespace FamilyHub.Api.Features.Calendar;

internal static class CalendarEventMappings
{
    internal static CalendarEventListItemDto ToListItemDto(this CalendarEvent calendarEvent) => new(
        calendarEvent.Id,
        calendarEvent.Title,
        calendarEvent.EventDate,
        calendarEvent.StartTime,
        calendarEvent.EndTime,
        calendarEvent.FamilyMemberId,
        calendarEvent.FamilyMember?.Name,
        calendarEvent.FamilyMember?.Color,
        calendarEvent.RecurrenceType,
        DeserializeRecurrenceDays(calendarEvent.RecurrenceDaysJson));

    internal static CalendarEventDetailsDto ToDetailsDto(this CalendarEvent calendarEvent) => new(
        calendarEvent.Id,
        calendarEvent.Title,
        calendarEvent.Description,
        calendarEvent.EventDate,
        calendarEvent.StartTime,
        calendarEvent.EndTime,
        calendarEvent.FamilyMemberId,
        calendarEvent.FamilyMember?.Name,
        calendarEvent.FamilyMember?.Color,
        calendarEvent.RecurrenceType,
        DeserializeRecurrenceDays(calendarEvent.RecurrenceDaysJson),
        calendarEvent.CreatedAtUtc,
        calendarEvent.UpdatedAtUtc);

    internal static CalendarEvent ToEntity(this CreateCalendarEventRequest request) => new()
    {
        Title = request.Title,
        Description = request.Description,
        EventDate = request.EventDate,
        StartTime = request.StartTime,
        EndTime = request.EndTime,
        FamilyMemberId = request.FamilyMemberId,
        RecurrenceType = request.RecurrenceType,
        RecurrenceDaysJson = SerializeRecurrenceDays(request.RecurrenceDays)
    };

    internal static void Apply(this CalendarEvent calendarEvent, UpdateCalendarEventRequest request)
    {
        calendarEvent.Title = request.Title;
        calendarEvent.Description = request.Description;
        calendarEvent.EventDate = request.EventDate;
        calendarEvent.StartTime = request.StartTime;
        calendarEvent.EndTime = request.EndTime;
        calendarEvent.FamilyMemberId = request.FamilyMemberId;
        calendarEvent.RecurrenceType = request.RecurrenceType;
        calendarEvent.RecurrenceDaysJson = SerializeRecurrenceDays(request.RecurrenceDays);
    }

    private static int[]? DeserializeRecurrenceDays(string? recurrenceDaysJson)
    {
        if (string.IsNullOrWhiteSpace(recurrenceDaysJson))
            return null;

        try
        {
            return JsonSerializer.Deserialize<int[]>(recurrenceDaysJson);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? SerializeRecurrenceDays(int[]? recurrenceDays)
        => recurrenceDays is null || recurrenceDays.Length == 0
            ? null
            : JsonSerializer.Serialize(recurrenceDays);
}