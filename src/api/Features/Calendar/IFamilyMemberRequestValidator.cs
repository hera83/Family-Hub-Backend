using FamilyHub.Api.Contracts.Calendar;

namespace FamilyHub.Api.Features.Calendar;

public interface IFamilyMemberRequestValidator
{
    void Validate(CreateFamilyMemberRequest request);
    void Validate(UpdateFamilyMemberRequest request);
}
