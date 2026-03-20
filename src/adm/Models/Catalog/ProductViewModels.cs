using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Adm.Models.Catalog;

public sealed class ProductListItemViewModel
{
    public Guid Id { get; init; }

    [Display(Name = "Navn")]
    public string Name { get; init; } = string.Empty;

    [Display(Name = "Kategori")]
    public string? ItemCategoryName { get; init; }

    [Display(Name = "Pris")]
    [DataType(DataType.Currency)]
    public decimal? Price { get; init; }

    [Display(Name = "Enhed")]
    public string? Unit { get; init; }

    [Display(Name = "Stoerrelse")]
    public string? SizeLabel { get; init; }

    [Display(Name = "Favorit")]
    public bool IsFavorite { get; init; }

    [Display(Name = "Basisvare")]
    public bool IsStaple { get; init; }
}

public sealed class ProductEditModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Navn er obligatorisk.")]
    [StringLength(200, ErrorMessage = "Navn ma maks vaere 200 tegn.")]
    [Display(Name = "Navn")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Kategori")]
    public Guid? ItemCategoryId { get; set; }

    [Display(Name = "Kategorinavn")]
    public string? ItemCategoryName { get; set; }

    [StringLength(2000, ErrorMessage = "Beskrivelse ma maks vaere 2000 tegn.")]
    [Display(Name = "Beskrivelse")]
    public string? Description { get; set; }

    [StringLength(1000, ErrorMessage = "Billede-URL ma maks vaere 1000 tegn.")]
    [Display(Name = "Billede URL")]
    [Url(ErrorMessage = "Billede URL er ugyldig.")]
    public string? ImageUrl { get; set; }

    [StringLength(50, ErrorMessage = "Enhed ma maks vaere 50 tegn.")]
    [Display(Name = "Enhed")]
    public string? Unit { get; set; }

    [StringLength(100, ErrorMessage = "Stoerrelsestekst ma maks vaere 100 tegn.")]
    [Display(Name = "Stoerrelse")]
    public string? SizeLabel { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Pris skal vaere 0 eller hoejere.")]
    [Display(Name = "Pris")]
    [DataType(DataType.Currency)]
    public decimal? Price { get; set; }

    [Display(Name = "Manuelt oprettet")]
    public bool IsManual { get; set; }

    [Display(Name = "Favorit")]
    public bool IsFavorite { get; set; }

    [Display(Name = "Basisvare")]
    public bool IsStaple { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Kalorier skal vaere 0 eller hoejere.")]
    [Display(Name = "Kalorier pr. 100 g")]
    public decimal? CaloriesPer100g { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Fedt skal vaere 0 eller hoejere.")]
    [Display(Name = "Fedt pr. 100 g")]
    public decimal? FatPer100g { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Kulhydrat skal vaere 0 eller hoejere.")]
    [Display(Name = "Kulhydrat pr. 100 g")]
    public decimal? CarbsPer100g { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Protein skal vaere 0 eller hoejere.")]
    [Display(Name = "Protein pr. 100 g")]
    public decimal? ProteinPer100g { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Fiber skal vaere 0 eller hoejere.")]
    [Display(Name = "Fiber pr. 100 g")]
    public decimal? FiberPer100g { get; set; }
}
