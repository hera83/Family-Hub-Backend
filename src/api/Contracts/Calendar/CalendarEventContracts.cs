using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Api.Contracts.Calendar;

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

public sealed class CreateCalendarEventRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; init; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; init; }

    [Required]
    public DateOnly EventDate { get; init; }

    public TimeOnly? StartTime { get; init; }

    public TimeOnly? EndTime { get; init; }

    public Guid? FamilyMemberId { get; init; }

    [StringLength(20)]
    public string? RecurrenceType { get; init; }

    public int[]? RecurrenceDays { get; init; }
}

public sealed class UpdateCalendarEventRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; init; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; init; }

    [Required]
    public DateOnly EventDate { get; init; }

    public TimeOnly? StartTime { get; init; }

    public TimeOnly? EndTime { get; init; }

    public Guid? FamilyMemberId { get; init; }

    [StringLength(20)]
    public string? RecurrenceType { get; init; }

    public int[]? RecurrenceDays { get; init; }
}
