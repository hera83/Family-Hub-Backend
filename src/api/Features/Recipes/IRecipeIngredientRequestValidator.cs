using FamilyHub.Api.Contracts.Recipes;

namespace FamilyHub.Api.Features.Recipes;

public interface IRecipeIngredientRequestValidator
{
    void Validate(CreateRecipeIngredientRequest request);
    void Validate(UpdateRecipeIngredientRequest request);
}
