namespace FamilyHub.Api.Entities.Recipes;

using FamilyHub.Api.Entities.Catalog;

/// <summary>
/// En ingredienslinje i en opskrift.
/// Kan enten pege på et katalogprodukt (ProductId) eller blot have et fritekst-navn.
/// </summary>
public class RecipeIngredient : BaseEntity
{
    public Guid RecipeId { get; set; }

    /// <summary>
    /// Valgfrit link til et katalogprodukt.
    /// Null = fritekst-ingrediens (se Name-feltet).
    /// DeleteBehavior.SetNull: produktet kan slettes uden at fjerne ingredienslinjen.
    /// </summary>
    public Guid? ProductId { get; set; }

    /// <summary>Fritekst-navn – bruges primært når ProductId er null.</summary>
    public string? Name { get; set; }

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    /// <summary>Basisingrediens der altid er hjemme.</summary>
    public bool IsStaple { get; set; }

    public int SortOrder { get; set; }

    // Navigation
    public Recipe Recipe { get; set; } = null!;
    public Product? Product { get; set; }
}
