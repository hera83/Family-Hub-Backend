using FamilyHub.Api.Contracts.Recipes;
using FamilyHub.Api.Features.Common;

namespace FamilyHub.Api.Features.Recipes;

internal sealed class RecipeRequestValidator : IRecipeRequestValidator
{
    public void Validate(RecipeListQueryRequest request)
        => PaginationValidator.Validate(request.Page, request.PageSize);

    public void Validate(CreateRecipeRequest request)
        => ValidateCore(request.Title, request.PrepTimeMinutes, request.WaitTimeMinutes);

    public void Validate(UpdateRecipeRequest request)
        => ValidateCore(request.Title, request.PrepTimeMinutes, request.WaitTimeMinutes);

    private static void ValidateCore(string? title, int? prepTimeMinutes, int? waitTimeMinutes)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title er påkrævet.");

        if (prepTimeMinutes < 0 || waitTimeMinutes < 0)
            throw new ArgumentException("PrepTimeMinutes og WaitTimeMinutes må ikke være negative.");
    }
}
