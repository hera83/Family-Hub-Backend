namespace FamilyHub.Api.Entities.Calendar;

/// <summary>
/// En kalenderbegivenhed tilknyttet et familiemedlem.
/// </summary>
public class CalendarEvent : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>
    /// Dato for begivenheden. DateOnly undgår tidszoneforvirring på betjeningspanelet.
    /// EF Core 8+ understøtter DateOnly med SQLite (lagres som TEXT "yyyy-MM-dd").
    /// </summary>
    public DateOnly EventDate { get; set; }

    /// <summary>
    /// Starttidspunkt. TimeOnly understøttet i EF Core 8+ med SQLite (lagres som TEXT).
    /// Null = heldagsbegivenhed.
    /// </summary>
    public TimeOnly? StartTime { get; set; }

    /// <summary>Sluttidspunkt. Null = åben slutning.</summary>
    public TimeOnly? EndTime { get; set; }

    // FK – SetNull: begivenheder bevares ved sletning af familiemedlem
    public Guid? FamilyMemberId { get; set; }

    /// <summary>Gentagelsestype: null / "None" / "Weekly" / "Monthly" / "Yearly".</summary>
    public string? RecurrenceType { get; set; }

    /// <summary>JSON-array af ugedage ved Weekly-gentagelse (fx "[1,3,5]" = man/ons/fre).</summary>
    public string? RecurrenceDaysJson { get; set; }

    // Navigation
    public FamilyMember? FamilyMember { get; set; }
}
