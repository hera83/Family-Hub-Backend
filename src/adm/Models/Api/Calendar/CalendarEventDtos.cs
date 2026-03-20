namespace FamilyHub.Adm.Models.Api.Calendar;

public sealed record CalendarEventListItemDto(
    Guid Id,
    string Title,
    DateOnly EventDate,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    Guid? FamilyMemberId,
    string? FamilyMemberName,
    string? FamilyMemberColor,
    string? RecurrenceType,
    int[]? RecurrenceDays
);

public sealed record CalendarEventDetailsDto(
    Guid Id,
    string Title,
    string? Description,
    DateOnly EventDate,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    Guid? FamilyMemberId,
    string? FamilyMemberName,
    string? FamilyMemberColor,
    string? RecurrenceType,
    int[]? RecurrenceDays,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);

public sealed class CalendarEventsQuery
{
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public Guid? FamilyMemberId { get; init; }
}

public sealed class CreateCalendarEventRequest
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateOnly EventDate { get; init; }
    public TimeOnly? StartTime { get; init; }
    public TimeOnly? EndTime { get; init; }
    public Guid? FamilyMemberId { get; init; }
    public string? RecurrenceType { get; init; }
    public int[]? RecurrenceDays { get; init; }
}

public sealed class UpdateCalendarEventRequest
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateOnly EventDate { get; init; }
    public TimeOnly? StartTime { get; init; }
    public TimeOnly? EndTime { get; init; }
    public Guid? FamilyMemberId { get; init; }
    public string? RecurrenceType { get; init; }
    public int[]? RecurrenceDays { get; init; }
}
