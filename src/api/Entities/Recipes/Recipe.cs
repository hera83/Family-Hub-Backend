namespace FamilyHub.Api.Entities.Recipes;

/// <summary>
/// En opskrift med tilhørende ingredienser og fremgangsmåde.
/// </summary>
public class Recipe : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public Guid? RecipeCategoryId { get; set; }

    public string? ImageUrl { get; set; }

    public string? Description { get; set; }

    public int? PrepTimeMinutes { get; set; }

    /// <summary>Ventetid (hævning, marinering, etc.) i minutter.</summary>
    public int? WaitTimeMinutes { get; set; }

    /// <summary>Fremgangsmåde – kan indeholde markdown.</summary>
    public string? Instructions { get; set; }

    /// <summary>Manuelt oprettet (ikke importeret fra ekstern kilde).</summary>
    public bool IsManual { get; set; }

    public bool IsFavorite { get; set; }

    // Navigation
    public RecipeCategory? RecipeCategory { get; set; }
    public ICollection<RecipeIngredient> Ingredients { get; set; } = [];
}
