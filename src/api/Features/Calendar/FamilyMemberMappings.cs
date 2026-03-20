using FamilyHub.Api.Contracts.Calendar;
using FamilyHub.Api.Entities.Calendar;

namespace FamilyHub.Api.Features.Calendar;

internal static class FamilyMemberMappings
{
    internal static FamilyMemberListItemDto ToListItemDto(this FamilyMember member) => new(
        member.Id,
        member.Name,
        member.Color);

    internal static FamilyMemberDetailsDto ToDetailsDto(this FamilyMember member) => new(
        member.Id,
        member.Name,
        member.Color,
        member.CreatedAtUtc,
        member.UpdatedAtUtc);

    internal static FamilyMember ToEntity(this CreateFamilyMemberRequest request) => new()
    {
        Name = request.Name,
        Color = request.Color
    };

    internal static void Apply(this FamilyMember member, UpdateFamilyMemberRequest request)
    {
        member.Name = request.Name;
        member.Color = request.Color;
    }
}