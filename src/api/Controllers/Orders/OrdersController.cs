using FamilyHub.Api.Contracts.Orders;
using FamilyHub.Api.Features.Orders;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHub.Api.Controllers.Orders;

/// <summary>
/// Endpoints for ordrehistorik og PDF-information.
/// </summary>
[Route("api/v1/orders")]
public sealed class OrdersController(IOrderService service) : ApiControllerBase
{
    /// <summary>
    /// Hent ordrer med filtrering og paginering.
    /// Understøtter status, page og pageSize.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] OrderListQueryRequest query, CancellationToken ct)
        => OkResponse(await service.GetAllAsync(query, ct));

    /// <summary>Hent én ordre med ordrelinjer via ID.</summary>
    [HttpGet("{id:guid}", Name = nameof(GetOrderById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(Guid id, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>
    /// Returnerer PDF-information for en ordre.
    /// V1 bruger simpel tekstbaseret lagring (PdfData) og kan udvides senere.
    /// </summary>
    [HttpGet("{id:guid}/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderPdfDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdf(Guid id, CancellationToken ct)
    {
        var pdf = await service.GetPdfByOrderIdAsync(id, ct);
        if (pdf is null)
            return NotFoundResponse("Ordre ikke fundet.");

        return OkResponse(pdf);
    }

    /// <summary>Slet en ordre og dens tilhørende ordrelinjer.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFoundResponse();
    }
}
