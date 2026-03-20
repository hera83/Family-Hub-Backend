using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Api.Contracts.Calendar;

public sealed record FamilyMemberListItemDto(
    Guid Id,
    string Name,
    string Color
);

public sealed record FamilyMemberDetailsDto(
    Guid Id,
    string Name,
    string Color,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);

public sealed class CreateFamilyMemberRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Color { get; init; } = string.Empty;
}

public sealed class UpdateFamilyMemberRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Color { get; init; } = string.Empty;
}
