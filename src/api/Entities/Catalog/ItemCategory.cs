namespace FamilyHub.Api.Entities.Catalog;

/// <summary>
/// En kategori der grupperer produkter i kataloget (fx "Mejeriprodukter").
/// </summary>
public class ItemCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    // Navigation
    public ICollection<Product> Products { get; set; } = [];
}
