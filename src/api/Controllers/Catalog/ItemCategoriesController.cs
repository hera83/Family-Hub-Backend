using FamilyHub.Api.Contracts.Catalog;
using FamilyHub.Api.Features.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHub.Api.Controllers.Catalog;

/// <summary>
/// CRUD-endpoints for varekategorier i kataloget.
/// </summary>
[Route("api/v1/catalog/categories")]
public sealed class ItemCategoriesController(IItemCategoryService service) : ApiControllerBase
{
    /// <summary>
    /// Hent alle varekategorier sorteret efter sortOrder og derefter name.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => OkResponse(await service.GetAllAsync(ct));

    /// <summary>Hent én varekategori via ID.</summary>
    [HttpGet("{id:guid}", Name = nameof(GetItemCategoryById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetItemCategoryById(Guid id, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>
    /// Opret ny varekategori.
    /// Eksempel request: { "name": "Kolonial", "sortOrder": 20 }
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateItemCategoryRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedResponse(nameof(GetItemCategoryById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Opdater en eksisterende varekategori.
    /// Eksempel request: { "name": "Mejeri", "sortOrder": 10 }
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateItemCategoryRequest request, CancellationToken ct)
    {
        var result = await service.UpdateAsync(id, request, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>
    /// Slet en varekategori.
    /// Sletning afvises, hvis kategorien stadig har produkter tilknyttet.
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
