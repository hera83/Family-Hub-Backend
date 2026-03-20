namespace FamilyHub.Api.Features.Catalog;

public interface ICatalogSyncRequestValidator
{
    DateTime ValidateAndParseSinceUtc(string? sinceUtc);
}
