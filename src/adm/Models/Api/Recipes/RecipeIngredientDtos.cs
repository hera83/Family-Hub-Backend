namespace FamilyHub.Adm.Models.Api.Recipes;

public sealed record RecipeIngredientDto(
    Guid Id,
    Guid RecipeId,
    Guid? ProductId,
    string? ProductName,
    string? ItemCategoryName,
    string? Name,
    decimal? Quantity,
    string? Unit,
    bool IsStaple,
    int SortOrder,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);

public sealed class CreateRecipeIngredientRequest
{
    public Guid? ProductId { get; init; }
    public string? Name { get; init; }
    public decimal? Quantity { get; init; }
    public string? Unit { get; init; }
    public bool IsStaple { get; init; }
    public int SortOrder { get; init; }
}

public sealed class UpdateRecipeIngredientRequest
{
    public Guid? ProductId { get; init; }
    public string? Name { get; init; }
    public decimal? Quantity { get; init; }
    public string? Unit { get; init; }
    public bool IsStaple { get; init; }
    public int SortOrder { get; init; }
}
