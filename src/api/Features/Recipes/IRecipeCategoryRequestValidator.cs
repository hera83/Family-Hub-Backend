using FamilyHub.Api.Contracts.Recipes;

namespace FamilyHub.Api.Features.Recipes;

public interface IRecipeCategoryRequestValidator
{
    void Validate(CreateRecipeCategoryRequest request);
    void Validate(UpdateRecipeCategoryRequest request);
}
