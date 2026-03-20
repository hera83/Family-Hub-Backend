using FamilyHub.Api.Contracts.Calendar;
using FamilyHub.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Features.Calendar;

public sealed class CalendarSyncService(
    FamilyHubDbContext db,
    ICalendarSyncRequestValidator validator) : ICalendarSyncService
{
    public async Task<CalendarSyncDto> GetCurrentStateAsync(CancellationToken ct = default)
    {
        var familyMembers = (await db.FamilyMembers
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        var calendarEvents = (await db.CalendarEvents
            .AsNoTracking()
            .Include(x => x.FamilyMember)
            .OrderBy(x => x.EventDate)
            .ThenBy(x => x.StartTime)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        return new CalendarSyncDto(DateTime.UtcNow, familyMembers, calendarEvents);
    }

    public async Task<CalendarChangesSinceDto> GetChangesSinceAsync(string? sinceUtc, CancellationToken ct = default)
    {
        var sinceUtcValue = validator.ValidateAndParseSinceUtc(sinceUtc);

        var familyMembers = (await db.FamilyMembers
            .AsNoTracking()
            .Where(x => (x.UpdatedAtUtc ?? x.CreatedAtUtc) > sinceUtcValue)
            .OrderBy(x => x.Name)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        var calendarEvents = (await db.CalendarEvents
            .AsNoTracking()
            .Include(x => x.FamilyMember)
            .Where(x => (x.UpdatedAtUtc ?? x.CreatedAtUtc) > sinceUtcValue)
            .OrderBy(x => x.EventDate)
            .ThenBy(x => x.StartTime)
            .ToListAsync(ct))
            .Select(x => x.ToDetailsDto())
            .ToList();

        return new CalendarChangesSinceDto(
            sinceUtcValue,
            DateTime.UtcNow,
            familyMembers,
            calendarEvents);
    }
}
