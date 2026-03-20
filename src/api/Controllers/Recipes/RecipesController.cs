using FamilyHub.Api.Contracts.Recipes;
using FamilyHub.Api.Features.Recipes;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHub.Api.Controllers.Recipes;

/// <summary>
/// CRUD-endpoints for opskrifter og deres ingredienser.
/// </summary>
[Route("api/v1/recipes/items")]
public sealed class RecipesController(IRecipeService service) : ApiControllerBase
{
    /// <summary>
    /// Hent liste af opskrifter med filtrering og paginering.
    /// Understøtter search, recipeCategoryId, favoritesOnly, page og pageSize.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] RecipeListQueryRequest query,
        CancellationToken ct)
        => OkResponse(await service.GetAllAsync(query, ct));

    /// <summary>Hent én opskrift uden ingrediensliste via ID.</summary>
    [HttpGet("{id:guid}", Name = nameof(GetRecipeById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRecipeById(Guid id, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>
    /// Hent den fulde visning af en opskrift inkl. kategorioplysninger og ingredienser.
    /// </summary>
    [HttpGet("{id:guid}/full")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRecipeFullById(Guid id, CancellationToken ct)
    {
        var result = await service.GetFullByIdAsync(id, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>
    /// Opret ny opskrift.
    /// Eksempel request: { "title": "Lasagne", "recipeCategoryId": null, "prepTimeMinutes": 30, "waitTimeMinutes": 45, "instructions": "Bag i ovnen", "isManual": true, "isFavorite": false }
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRecipeRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedResponse(nameof(GetRecipeById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Opdater en eksisterende opskrift.
    /// Eksempel request: { "title": "Lasagne", "recipeCategoryId": null, "prepTimeMinutes": 35, "waitTimeMinutes": 40, "instructions": "Dæk med ost og bag", "isManual": true, "isFavorite": true }
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRecipeRequest request, CancellationToken ct)
    {
        var result = await service.UpdateAsync(id, request, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>Slet en opskrift (ingredienser slettes automatisk via cascade).</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFoundResponse();
    }

    // ─── Ingredienser ─────────────────────────────────────────────────────────

    /// <summary>
    /// Hent ingredienser for en opskrift sorteret efter sortOrder og derefter navn.
    /// </summary>
    [HttpGet("{id:guid}/ingredients")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIngredients(Guid id, CancellationToken ct)
    {
        var result = await service.GetIngredientsAsync(id, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>
    /// Tilføj ingrediens til en opskrift.
    /// Eksempel request: { "productId": null, "name": "Salt", "quantity": 1, "unit": "tsk", "isStaple": true, "sortOrder": 10 }
    /// </summary>
    [HttpPost("{id:guid}/ingredients")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddIngredient(
        Guid id, [FromBody] CreateRecipeIngredientRequest request, CancellationToken ct)
    {
        var created = await service.AddIngredientAsync(id, request, ct);
        return CreatedActionResponse(nameof(GetIngredients), new { id }, created);
    }

    /// <summary>
    /// Opdater en eksisterende ingredienslinje under den angivne opskrift.
    /// </summary>
    [HttpPut("{id:guid}/ingredients/{ingredientId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateIngredient(
        Guid id, Guid ingredientId, [FromBody] UpdateRecipeIngredientRequest request, CancellationToken ct)
    {
        var result = await service.UpdateIngredientAsync(id, ingredientId, request, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>
    /// Slet en ingredienslinje under den angivne opskrift.
    /// </summary>
    [HttpDelete("{id:guid}/ingredients/{ingredientId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteIngredient(Guid id, Guid ingredientId, CancellationToken ct)
    {
        var deleted = await service.DeleteIngredientAsync(id, ingredientId, ct);
        return deleted ? NoContent() : NotFoundResponse();
    }
}
