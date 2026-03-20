namespace FamilyHub.Api.Entities.Calendar;

/// <summary>
/// Et familiemedlem der kan tilknyttes kalenderbegivenheder.
/// </summary>
public class FamilyMember : BaseEntity
{
    /// <summary>Fuldt navn på familiemedlemmet.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Farve brugt til visning i UI (hex-format, fx "#FF5733").</summary>
    public string Color { get; set; } = "#000000";

    // Navigation
    public ICollection<CalendarEvent> CalendarEvents { get; set; } = [];
}
