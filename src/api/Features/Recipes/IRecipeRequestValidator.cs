using FamilyHub.Api.Contracts.Recipes;

namespace FamilyHub.Api.Features.Recipes;

public interface IRecipeRequestValidator
{
    void Validate(RecipeListQueryRequest request);
    void Validate(CreateRecipeRequest request);
    void Validate(UpdateRecipeRequest request);
}
