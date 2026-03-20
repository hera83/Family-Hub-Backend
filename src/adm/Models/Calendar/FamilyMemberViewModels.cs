using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Adm.Models.Calendar;

public sealed class FamilyMemberListItemViewModel
{
    public Guid Id { get; init; }

    [Display(Name = "Navn")]
    public string Name { get; init; } = string.Empty;

    [Display(Name = "Farve")]
    public string Color { get; init; } = string.Empty;
}

public sealed class FamilyMemberEditModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Navn er obligatorisk.")]
    [StringLength(100, ErrorMessage = "Navn ma maks vaere 100 tegn.")]
    [Display(Name = "Navn")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Farve er obligatorisk.")]
    [StringLength(50, ErrorMessage = "Farve ma maks vaere 50 tegn.")]
    [Display(Name = "Farvekode")]
    public string Color { get; set; } = string.Empty;
}
