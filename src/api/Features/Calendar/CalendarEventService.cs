using FamilyHub.Api.Contracts.Calendar;
using FamilyHub.Api.Entities.Calendar;
using FamilyHub.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Features.Calendar;

/// <summary>
/// Håndterer CRUD for kalenderbegivenheder med optional datofiltrering.
/// </summary>
public sealed class CalendarEventService(
    FamilyHubDbContext db,
    ICalendarEventRequestValidator validator) : ICalendarEventService
{
    public async Task<IEnumerable<CalendarEventListItemDto>> GetAllAsync(
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        Guid? familyMemberId = null,
        CancellationToken ct = default)
    {
        validator.ValidateFilter(fromDate, toDate);

        var query = db.CalendarEvents
            .AsNoTracking()
            .Include(e => e.FamilyMember)
            .AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(e => e.EventDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(e => e.EventDate <= toDate.Value);

        if (familyMemberId.HasValue)
            query = query.Where(e => e.FamilyMemberId == familyMemberId.Value);

        var events = await query
            .OrderBy(e => e.EventDate)
            .ThenBy(e => e.StartTime)
            .ToListAsync(ct);

        return events.Select(e => e.ToListItemDto()).ToList();
    }

    public async Task<CalendarEventDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var ev = await db.CalendarEvents
            .AsNoTracking()
            .Include(e => e.FamilyMember)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        return ev?.ToDetailsDto();
    }

    public async Task<CalendarEventDetailsDto> CreateAsync(CreateCalendarEventRequest request, CancellationToken ct = default)
    {
        validator.Validate(request);

        if (request.FamilyMemberId.HasValue)
        {
            var memberExists = await db.FamilyMembers
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.FamilyMemberId.Value, ct);

            if (!memberExists)
                throw new ArgumentException("Det angivne familyMemberId findes ikke.");
        }

        var ev = request.ToEntity();

        db.CalendarEvents.Add(ev);
        await db.SaveChangesAsync(ct);

        await db.Entry(ev).Reference(e => e.FamilyMember).LoadAsync(ct);
        return ev.ToDetailsDto();
    }

    public async Task<CalendarEventDetailsDto?> UpdateAsync(Guid id, UpdateCalendarEventRequest request, CancellationToken ct = default)
    {
        validator.Validate(request);

        if (request.FamilyMemberId.HasValue)
        {
            var memberExists = await db.FamilyMembers
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.FamilyMemberId.Value, ct);

            if (!memberExists)
                throw new ArgumentException("Det angivne familyMemberId findes ikke.");
        }

        var ev = await db.CalendarEvents
            .FirstOrDefaultAsync(e => e.Id == id, ct);
        if (ev is null) return null;

        ev.Apply(request);

        await db.SaveChangesAsync(ct);

        if (ev.FamilyMemberId is null)
        {
            ev.FamilyMember = null;
        }
        else
        {
            db.Entry(ev).Reference(e => e.FamilyMember).IsLoaded = false;
            await db.Entry(ev).Reference(e => e.FamilyMember).LoadAsync(ct);
        }

        return ev.ToDetailsDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var ev = await db.CalendarEvents.FindAsync([id], ct);
        if (ev is null) return false;

        db.CalendarEvents.Remove(ev);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
