using FamilyHub.Api.Contracts.Recipes;

namespace FamilyHub.Api.Features.Recipes;

internal sealed class RecipeCategoryRequestValidator : IRecipeCategoryRequestValidator
{
    public void Validate(CreateRecipeCategoryRequest request)
        => ValidateCore(request.Name, request.SortOrder);

    public void Validate(UpdateRecipeCategoryRequest request)
        => ValidateCore(request.Name, request.SortOrder);

    private static void ValidateCore(string? name, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name er påkrævet.");

        if (sortOrder < 0)
            throw new ArgumentException("SortOrder må ikke være negativ.");
    }
}
