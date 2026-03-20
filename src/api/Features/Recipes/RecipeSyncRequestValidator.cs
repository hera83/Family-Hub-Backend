using FamilyHub.Api.Features.Common;

namespace FamilyHub.Api.Features.Recipes;

internal sealed class RecipeSyncRequestValidator : IRecipeSyncRequestValidator
{
    public DateTime ValidateAndParseSinceUtc(string? sinceUtc)
        => SinceUtcParser.ValidateAndParse(sinceUtc);
}
