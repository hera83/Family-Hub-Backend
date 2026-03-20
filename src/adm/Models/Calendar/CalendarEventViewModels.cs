using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Adm.Models.Calendar;

public sealed class CalendarEventListItemViewModel
{
    public Guid Id { get; init; }

    [Display(Name = "Titel")]
    public string Title { get; init; } = string.Empty;

    [Display(Name = "Dato")]
    public DateOnly EventDate { get; init; }

    [Display(Name = "Start")]
    public TimeOnly? StartTime { get; init; }

    [Display(Name = "Slut")]
    public TimeOnly? EndTime { get; init; }

    [Display(Name = "Familiemedlem")]
    public string? FamilyMemberName { get; init; }

    [Display(Name = "Familiemedlem farve")]
    public string? FamilyMemberColor { get; init; }

    [Display(Name = "Gentagelse")]
    public string? RecurrenceType { get; init; }

    [Display(Name = "Gentagelsesdage")]
    public string? RecurrenceDaysDisplay { get; init; }
}

public sealed class CalendarEventEditModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Titel er obligatorisk.")]
    [StringLength(200, ErrorMessage = "Titel ma maks vaere 200 tegn.")]
    [Display(Name = "Titel")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Beskrivelse ma maks vaere 2000 tegn.")]
    [Display(Name = "Beskrivelse")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Dato er obligatorisk.")]
    [Display(Name = "Dato")]
    [DataType(DataType.Date)]
    public DateOnly EventDate { get; set; }

    [Display(Name = "Starttid")]
    [DataType(DataType.Time)]
    public TimeOnly? StartTime { get; set; }

    [Display(Name = "Sluttid")]
    [DataType(DataType.Time)]
    public TimeOnly? EndTime { get; set; }

    [Display(Name = "Familiemedlem")]
    public Guid? FamilyMemberId { get; set; }

    [Display(Name = "Gentagelsestype")]
    [StringLength(20, ErrorMessage = "Gentagelsestype ma maks vaere 20 tegn.")]
    public string? RecurrenceType { get; set; }

    [Display(Name = "Gentagelsesdage")]
    public List<int> RecurrenceDays { get; set; } = [];
}
