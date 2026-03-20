namespace FamilyHub.Api.Entities.Recipes;

/// <summary>
/// En kategori der grupperer opskrifter (fx "Aftensmad", "Bagværk").
/// </summary>
public class RecipeCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    // Navigation
    public ICollection<Recipe> Recipes { get; set; } = [];
}
