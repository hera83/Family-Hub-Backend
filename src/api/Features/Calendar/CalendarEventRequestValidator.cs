using FamilyHub.Api.Contracts.Calendar;

namespace FamilyHub.Api.Features.Calendar;

internal sealed class CalendarEventRequestValidator : ICalendarEventRequestValidator
{
    private static readonly HashSet<string> AllowedRecurrenceTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "None",
        "Weekly",
        "Monthly",
        "Yearly"
    };

    public void ValidateFilter(DateOnly? fromDate, DateOnly? toDate)
    {
        if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
            throw new ArgumentException("fromDate må ikke være senere end toDate.");
    }

    public void Validate(CreateCalendarEventRequest request)
        => ValidateCore(
            request.Title,
            request.EventDate,
            request.StartTime,
            request.EndTime,
            request.RecurrenceType,
            request.RecurrenceDays);

    public void Validate(UpdateCalendarEventRequest request)
        => ValidateCore(
            request.Title,
            request.EventDate,
            request.StartTime,
            request.EndTime,
            request.RecurrenceType,
            request.RecurrenceDays);

    private static void ValidateCore(
        string? title,
        DateOnly eventDate,
        TimeOnly? startTime,
        TimeOnly? endTime,
        string? recurrenceType,
        int[]? recurrenceDays)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title er påkrævet.");

        if (eventDate == default)
            throw new ArgumentException("EventDate er påkrævet.");

        if (startTime.HasValue && endTime.HasValue && endTime <= startTime)
            throw new ArgumentException("EndTime skal være senere end StartTime, når begge er angivet.");

        if (!string.IsNullOrWhiteSpace(recurrenceType) && !AllowedRecurrenceTypes.Contains(recurrenceType))
            throw new ArgumentException("RecurrenceType må kun være None, Weekly, Monthly, Yearly eller null.");

        if (recurrenceDays is { Length: > 0 })
        {
            if (!string.Equals(recurrenceType, "Weekly", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("RecurrenceDays må kun angives, når RecurrenceType er Weekly.");

            if (recurrenceDays.Any(day => day is < 1 or > 7))
                throw new ArgumentException("RecurrenceDays må kun indeholde værdier fra 1 til 7.");
        }

        if (string.Equals(recurrenceType, "Weekly", StringComparison.OrdinalIgnoreCase)
            && (recurrenceDays is null || recurrenceDays.Length == 0))
        {
            throw new ArgumentException("RecurrenceDays er påkrævet, når RecurrenceType er Weekly.");
        }

        if (string.Equals(recurrenceType, "None", StringComparison.OrdinalIgnoreCase)
            && recurrenceDays is { Length: > 0 })
        {
            throw new ArgumentException("RecurrenceDays må ikke angives, når RecurrenceType er None.");
        }
    }
}
