using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Adm.Models.Recipes;

public sealed class RecipeListItemViewModel
{
    public Guid Id { get; init; }

    [Display(Name = "Titel")]
    public string Title { get; init; } = string.Empty;

    [Display(Name = "Kategori")]
    public string? RecipeCategoryName { get; init; }

    [Display(Name = "Forberedelse (min)")]
    public int? PrepTimeMinutes { get; init; }

    [Display(Name = "Ventetid (min)")]
    public int? WaitTimeMinutes { get; init; }

    [Display(Name = "Favorit")]
    public bool IsFavorite { get; init; }
}

public sealed class RecipeEditModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Titel er obligatorisk.")]
    [StringLength(200, ErrorMessage = "Titel ma maks vaere 200 tegn.")]
    [Display(Name = "Titel")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Kategori")]
    public Guid? RecipeCategoryId { get; set; }

    [Display(Name = "Kategorinavn")]
    public string? RecipeCategoryName { get; set; }

    [StringLength(1000, ErrorMessage = "Billede-URL ma maks vaere 1000 tegn.")]
    [Display(Name = "Billede URL")]
    [Url(ErrorMessage = "Billede URL er ugyldig.")]
    public string? ImageUrl { get; set; }

    [StringLength(4000, ErrorMessage = "Beskrivelse ma maks vaere 4000 tegn.")]
    [Display(Name = "Beskrivelse")]
    public string? Description { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Forberedelsestid skal vaere 0 eller hoejere.")]
    [Display(Name = "Forberedelse (min)")]
    public int? PrepTimeMinutes { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Ventetid skal vaere 0 eller hoejere.")]
    [Display(Name = "Ventetid (min)")]
    public int? WaitTimeMinutes { get; set; }

    [Display(Name = "Fremgangsmaade")]
    public string? Instructions { get; set; }

    [Display(Name = "Manuelt oprettet")]
    public bool IsManual { get; set; }

    [Display(Name = "Favorit")]
    public bool IsFavorite { get; set; }
}

public sealed class RecipeFullViewModel
{
    [Required]
    [Display(Name = "Opskrift")]
    public RecipeEditModel Recipe { get; set; } = new();

    [Display(Name = "Ingredienser")]
    public IReadOnlyList<RecipeIngredientViewModel> Ingredients { get; set; } = [];
}
