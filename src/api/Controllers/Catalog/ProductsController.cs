using FamilyHub.Api.Contracts.Catalog;
using FamilyHub.Api.Features.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHub.Api.Controllers.Catalog;

/// <summary>
/// CRUD-endpoints for produkter i kataloget.
/// </summary>
[Route("api/v1/catalog/products")]
public sealed class ProductsController(IProductService service) : ApiControllerBase
{
    /// <summary>
    /// Hent produkter med filtrering og paginering.
    /// Understøtter search, itemCategoryId, favoritesOnly, staplesOnly, page og pageSize.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] ProductListQueryRequest query, CancellationToken ct)
        => OkResponse(await service.GetAllAsync(query, ct));

    /// <summary>Hent ét produkt via ID.</summary>
    [HttpGet("{id:guid}", Name = nameof(GetProductById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>
    /// Opret nyt produkt.
    /// Eksempel request: { "name": "Havregryn", "itemCategoryId": null, "unit": "g", "price": 18.5, "isManual": true, "isFavorite": false, "isStaple": true }
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedResponse(nameof(GetProductById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Opdater et eksisterende produkt.
    /// Eksempel request: { "name": "Skyr", "itemCategoryId": null, "description": "Neutral skyr", "unit": "g", "sizeLabel": "1 kg", "price": 24.95, "isManual": true, "isFavorite": true, "isStaple": false }
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var result = await service.UpdateAsync(id, request, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>Slet et produkt.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFoundResponse();
    }
}
