using FamilyHub.Api.Features.Common;

namespace FamilyHub.Api.Features.Sync;

internal sealed class SyncRequestValidator : ISyncRequestValidator
{
    public DateTime ValidateAndParseSinceUtc(string? sinceUtc)
        => SinceUtcParser.ValidateAndParse(sinceUtc);
}
