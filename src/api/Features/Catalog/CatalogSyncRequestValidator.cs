using FamilyHub.Api.Features.Common;

namespace FamilyHub.Api.Features.Catalog;

internal sealed class CatalogSyncRequestValidator : ICatalogSyncRequestValidator
{
    public DateTime ValidateAndParseSinceUtc(string? sinceUtc)
        => SinceUtcParser.ValidateAndParse(sinceUtc);
}
