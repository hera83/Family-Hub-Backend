namespace FamilyHub.Api.Features.Calendar;

public interface ICalendarSyncRequestValidator
{
    DateTime ValidateAndParseSinceUtc(string? sinceUtc);
}
