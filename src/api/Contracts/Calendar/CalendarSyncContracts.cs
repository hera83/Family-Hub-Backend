namespace FamilyHub.Api.Contracts.Calendar;

public sealed record CalendarSyncDto(
    DateTime GeneratedAtUtc,
    IReadOnlyList<FamilyMemberDetailsDto> FamilyMembers,
    IReadOnlyList<CalendarEventDetailsDto> CalendarEvents
);

public sealed record CalendarChangesSinceDto(
    DateTime SinceUtc,
    DateTime GeneratedAtUtc,
    IReadOnlyList<FamilyMemberDetailsDto> FamilyMembers,
    IReadOnlyList<CalendarEventDetailsDto> CalendarEvents
);
