using System.Text.RegularExpressions;
using FamilyHub.Api.Contracts.Calendar;

namespace FamilyHub.Api.Features.Calendar;

internal sealed partial class FamilyMemberRequestValidator : IFamilyMemberRequestValidator
{
    private static readonly Regex HexColorRegex = HexColorPattern();

    public void Validate(CreateFamilyMemberRequest request)
        => ValidateCore(request.Name, request.Color);

    public void Validate(UpdateFamilyMemberRequest request)
        => ValidateCore(request.Name, request.Color);

    private static void ValidateCore(string? name, string? color)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name er påkrævet.");

        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("Color er påkrævet.");

        if (!HexColorRegex.IsMatch(color))
            throw new ArgumentException("Color skal være en gyldig hex-farve, fx #22C55E.");
    }

    [GeneratedRegex("^#(?:[0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$")]
    private static partial Regex HexColorPattern();
}
