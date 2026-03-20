using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Api.Contracts.Recipes;

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

    [StringLength(200)]
    public string? Name { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? Quantity { get; init; }

    [StringLength(50)]
    public string? Unit { get; init; }

    public bool IsStaple { get; init; }

    [Range(0, int.MaxValue)]
    public int SortOrder { get; init; }
}

public sealed class UpdateRecipeIngredientRequest
{
    public Guid? ProductId { get; init; }

    [StringLength(200)]
    public string? Name { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? Quantity { get; init; }

    [StringLength(50)]
    public string? Unit { get; init; }

    public bool IsStaple { get; init; }

    [Range(0, int.MaxValue)]
    public int SortOrder { get; init; }
}
