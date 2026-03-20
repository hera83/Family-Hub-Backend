using FamilyHub.Api.Contracts.Recipes;
using FamilyHub.Api.Features.Recipes;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHub.Api.Controllers.Recipes;

/// <summary>
/// CRUD-endpoints for opskriftskategorier.
/// </summary>
[Route("api/v1/recipes/categories")]
public sealed class RecipeCategoriesController(IRecipeCategoryService service) : ApiControllerBase
{
    /// <summary>
    /// Hent alle opskriftskategorier sorteret efter sortOrder og name.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => OkResponse(await service.GetAllAsync(ct));

    /// <summary>Hent én opskriftskategori via ID.</summary>
    [HttpGet("{id:guid}", Name = nameof(GetRecipeCategoryById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRecipeCategoryById(Guid id, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>
    /// Opret ny opskriftskategori.
    /// Eksempel request: { "name": "Aftensmad", "sortOrder": 10 }
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRecipeCategoryRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedResponse(nameof(GetRecipeCategoryById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Opdater en eksisterende opskriftskategori.
    /// Eksempel request: { "name": "Bagning", "sortOrder": 20 }
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRecipeCategoryRequest request, CancellationToken ct)
    {
        var result = await service.UpdateAsync(id, request, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>
    /// Slet en opskriftskategori.
    /// Sletning afvises, hvis der stadig findes opskrifter i kategorien.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFoundResponse();
    }
}
