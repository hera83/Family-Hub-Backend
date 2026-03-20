using FamilyHub.Api.Contracts.Calendar;
using FamilyHub.Api.Entities.Calendar;
using FamilyHub.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Features.Calendar;

/// <summary>
/// Håndterer CRUD for familiemedlemmer.
/// </summary>
public sealed class FamilyMemberService(
    FamilyHubDbContext db,
    IFamilyMemberRequestValidator validator) : IFamilyMemberService
{
    public async Task<IEnumerable<FamilyMemberListItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        var members = await db.FamilyMembers
            .AsNoTracking()
            .OrderBy(m => m.Name)
            .ToListAsync(ct);

        return members.Select(m => m.ToListItemDto()).ToList();
    }

    public async Task<FamilyMemberDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var member = await db.FamilyMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, ct);

        return member?.ToDetailsDto();
    }

    public async Task<FamilyMemberDetailsDto> CreateAsync(CreateFamilyMemberRequest request, CancellationToken ct = default)
    {
        validator.Validate(request);

        var member = request.ToEntity();

        db.FamilyMembers.Add(member);
        await db.SaveChangesAsync(ct);

        return member.ToDetailsDto();
    }

    public async Task<FamilyMemberDetailsDto?> UpdateAsync(Guid id, UpdateFamilyMemberRequest request, CancellationToken ct = default)
    {
        validator.Validate(request);

        var member = await db.FamilyMembers.FindAsync([id], ct);
        if (member is null) return null;

        member.Apply(request);

        await db.SaveChangesAsync(ct);
        return member.ToDetailsDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var member = await db.FamilyMembers.FindAsync([id], ct);
        if (member is null) return false;

        db.FamilyMembers.Remove(member);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
