namespace FamilyHub.Api.Features.Recipes;

public interface IRecipeSyncRequestValidator
{
    DateTime ValidateAndParseSinceUtc(string? sinceUtc);
}
