namespace FamilyHub.Api.Entities.Catalog;

using FamilyHub.Api.Entities.Recipes;

/// <summary>
/// Et produkt i kataloget – bruges på indkøbslisten og som ingrediens i opskrifter.
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public Guid? ItemCategoryId { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? Unit { get; set; }

    public string? SizeLabel { get; set; }

    public decimal? Price { get; set; }

    /// <summary>Manuelt oprettet (ikke importeret fra ekstern kilde).</summary>
    public bool IsManual { get; set; }

    public bool IsFavorite { get; set; }

    /// <summary>Basisvare der altid er på lager.</summary>
    public bool IsStaple { get; set; }

    // Næringsindhold per 100 g
    public decimal? CaloriesPer100g { get; set; }
    public decimal? FatPer100g { get; set; }
    public decimal? CarbsPer100g { get; set; }
    public decimal? ProteinPer100g { get; set; }
    public decimal? FiberPer100g { get; set; }

    // Navigation
    public ItemCategory? ItemCategory { get; set; }

    // Et produkt kan indgå i mange opskrift-ingredienser
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
}
