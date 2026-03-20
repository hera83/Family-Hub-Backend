using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Adm.Models.Recipes;

public sealed class RecipeIngredientViewModel
{
    public Guid Id { get; init; }
    public Guid RecipeId { get; init; }

    [Display(Name = "Produkt")]
    public string? ProductName { get; init; }

    [Display(Name = "Kategori")]
    public string? ItemCategoryName { get; init; }

    [Display(Name = "Ingrediens")]
    public string? Name { get; init; }

    [Display(Name = "Maengde")]
    public decimal? Quantity { get; init; }

    [Display(Name = "Enhed")]
    public string? Unit { get; init; }

    [Display(Name = "Basisvare")]
    public bool IsStaple { get; init; }

    [Display(Name = "Sortering")]
    public int SortOrder { get; init; }
}

public sealed class RecipeIngredientEditModel : IValidatableObject
{
    public Guid? Id { get; set; }
    public Guid? RecipeId { get; set; }

    [Display(Name = "Produkt")]
    public Guid? ProductId { get; set; }

    [StringLength(200, ErrorMessage = "Navn ma maks vaere 200 tegn.")]
    [Display(Name = "Ingrediensnavn")]
    public string? Name { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Maengde skal vaere 0 eller hoejere.")]
    [Display(Name = "Maengde")]
    public decimal? Quantity { get; set; }

    [StringLength(50, ErrorMessage = "Enhed ma maks vaere 50 tegn.")]
    [Display(Name = "Enhed")]
    public string? Unit { get; set; }

    [Display(Name = "Basisvare")]
    public bool IsStaple { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Sortering skal vaere 0 eller hoejere.")]
    [Display(Name = "Sortering")]
    public int SortOrder { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!ProductId.HasValue && string.IsNullOrWhiteSpace(Name))
        {
            yield return new ValidationResult(
                "Vaelg et produkt eller angiv et ingrediensnavn.",
                [nameof(ProductId), nameof(Name)]);
        }
    }
}
