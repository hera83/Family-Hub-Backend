using FamilyHub.Api.Features.Common;

namespace FamilyHub.Api.Features.Calendar;

internal sealed class CalendarSyncRequestValidator : ICalendarSyncRequestValidator
{
    public DateTime ValidateAndParseSinceUtc(string? sinceUtc)
        => SinceUtcParser.ValidateAndParse(sinceUtc);
}
