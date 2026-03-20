using FamilyHub.Api.Contracts.Recipes;

namespace FamilyHub.Api.Features.Recipes;

internal sealed class RecipeIngredientRequestValidator : IRecipeIngredientRequestValidator
{
    public void Validate(CreateRecipeIngredientRequest request)
        => ValidateCore(request.ProductId, request.Name, request.Quantity);

    public void Validate(UpdateRecipeIngredientRequest request)
        => ValidateCore(request.ProductId, request.Name, request.Quantity);

    private static void ValidateCore(Guid? productId, string? name, decimal? quantity)
    {
        if (!productId.HasValue && string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Mindst én af ProductId eller Name skal være angivet.");

        if (quantity < 0)
            throw new ArgumentException("Quantity må ikke være negativ.");
    }
}
