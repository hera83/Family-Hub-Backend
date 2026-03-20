using FamilyHub.Api.Contracts.Catalog;

namespace FamilyHub.Api.Features.Catalog;

internal sealed class ItemCategoryRequestValidator : IItemCategoryRequestValidator
{
    public void Validate(CreateItemCategoryRequest request)
        => ValidateCore(request.Name, request.SortOrder);

    public void Validate(UpdateItemCategoryRequest request)
        => ValidateCore(request.Name, request.SortOrder);

    private static void ValidateCore(string? name, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name er påkrævet.");

        if (sortOrder < 0)
            throw new ArgumentException("SortOrder må ikke være negativ.");
    }
}
