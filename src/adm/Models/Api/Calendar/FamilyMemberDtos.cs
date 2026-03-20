namespace FamilyHub.Adm.Models.Api.Calendar;

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
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
}

public sealed class UpdateFamilyMemberRequest
{
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
}
