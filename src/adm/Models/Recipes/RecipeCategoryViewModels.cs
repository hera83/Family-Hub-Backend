using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Adm.Models.Recipes;

public sealed class RecipeCategoryListItemViewModel
{
    public Guid Id { get; init; }

    [Display(Name = "Navn")]
    public string Name { get; init; } = string.Empty;

    [Display(Name = "Sortering")]
    public int SortOrder { get; init; }
}

public sealed class RecipeCategoryEditModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Navn er obligatorisk.")]
    [StringLength(100, ErrorMessage = "Navn ma maks vaere 100 tegn.")]
    [Display(Name = "Navn")]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Sortering skal vaere 0 eller hoejere.")]
    [Display(Name = "Sortering")]
    public int SortOrder { get; set; }
}
