namespace FamilyHub.Api.Entities;

/// <summary>
/// Basisklasse for alle entiteter.
/// Bruger Guid som ID for URL-sikkerhed og distribueret kompatibilitet.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Oprettelsestidspunkt i UTC.</summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Tidspunkt for seneste opdatering i UTC. Null hvis aldrig opdateret.</summary>
    public DateTime? UpdatedAtUtc { get; set; }
}
