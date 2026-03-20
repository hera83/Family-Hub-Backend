using FamilyHub.Api.Contracts.Calendar;
using FamilyHub.Api.Features.Calendar;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHub.Api.Controllers.Calendar;

/// <summary>
/// CRUD-endpoints for familiemedlemmer.
/// </summary>
[Route("api/v1/calendar/members")]
public sealed class FamilyMembersController(IFamilyMemberService service) : ApiControllerBase
{
    /// <summary>
    /// Hent alle familiemedlemmer sorteret alfabetisk på navn.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => OkResponse(await service.GetAllAsync(ct));

    /// <summary>Hent ét familiemedlem via ID.</summary>
    [HttpGet("{id:guid}", Name = nameof(GetFamilyMemberById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFamilyMemberById(Guid id, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>
    /// Opret nyt familiemedlem.
    /// Eksempel request: { "name": "Maja", "color": "#22C55E" }
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateFamilyMemberRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedResponse(nameof(GetFamilyMemberById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Opdater navn og farve på et eksisterende familiemedlem.
    /// Eksempel request: { "name": "Maja Sørensen", "color": "#0EA5E9" }
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFamilyMemberRequest request, CancellationToken ct)
    {
        var result = await service.UpdateAsync(id, request, ct);
        return result is null ? NotFoundResponse() : OkResponse(result);
    }

    /// <summary>
    /// Slet et familiemedlem.
    /// Eksisterende kalenderbegivenheder bevares og mister blot relationen til medlemmet.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFoundResponse();
    }
}
