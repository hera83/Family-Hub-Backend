namespace FamilyHub.Api.Features.Sync;

public interface ISyncRequestValidator
{
    DateTime ValidateAndParseSinceUtc(string? sinceUtc);
}
