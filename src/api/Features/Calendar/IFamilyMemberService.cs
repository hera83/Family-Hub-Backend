using FamilyHub.Api.Contracts.Calendar;

namespace FamilyHub.Api.Features.Calendar;

/// <summary>
/// Service-kontrakt for familiemedlemmer.
/// Implementeres i <see cref="FamilyMemberService"/>.
/// </summary>
public interface IFamilyMemberService
{
    Task<IEnumerable<FamilyMemberListItemDto>> GetAllAsync(CancellationToken ct = default);
    Task<FamilyMemberDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<FamilyMemberDetailsDto> CreateAsync(CreateFamilyMemberRequest request, CancellationToken ct = default);
    Task<FamilyMemberDetailsDto?> UpdateAsync(Guid id, UpdateFamilyMemberRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
