using FamilyHub.Api.Features.Recipes;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHub.Api.Controllers.Recipes;

/// <summary>
/// Sync-endpoints for Recipes-featuret.
/// </summary>
[Route("api/v1/recipes/sync")]
public sealed class RecipeSyncController(IRecipeSyncService service) : ApiControllerBase
{
    /// <summary>
    /// Returnerer den samlede recipes-payload til initial indlæsning.
    /// Svaret indeholder recipeCategories, recipes, recipeIngredients og generatedAtUtc.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken ct)
        => OkResponse(await service.GetCurrentStateAsync(ct));

    /// <summary>
    /// Returnerer kun opskriftsdata ændret efter et givent UTC-tidspunkt.
    /// Eksempel: /api/v1/recipes/sync/changes?sinceUtc=2026-03-19T08:30:00Z
    /// </summary>
    [HttpGet("changes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetChanges([FromQuery] string? sinceUtc, CancellationToken ct)
        => OkResponse(await service.GetChangesSinceAsync(sinceUtc, ct));
}
