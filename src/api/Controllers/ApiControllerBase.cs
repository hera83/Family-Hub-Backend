using Microsoft.AspNetCore.Mvc;

namespace FamilyHub.Api.Controllers;

/// <summary>
/// Basiscontroller der tilføjer fælles rute-prefix og giver adgang til fælles response-hjælpere.
/// Alle controllers arver fra denne.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Returnerer 200 OK med payload direkte.</summary>
    protected IActionResult OkResponse<T>(T data) => Ok(data);

    /// <summary>Returnerer 201 Created med payload direkte.</summary>
    protected IActionResult CreatedResponse<T>(string routeName, object routeValues, T data)
        => CreatedAtRoute(routeName, routeValues, data);

    /// <summary>Returnerer 201 Created (CreatedAtAction) med payload direkte.</summary>
    protected IActionResult CreatedActionResponse<T>(string actionName, object routeValues, T data)
        => CreatedAtAction(actionName, routeValues, data);

    /// <summary>Returnerer 404 Not Found med ProblemDetails.</summary>
    protected IActionResult NotFoundResponse(string message = "Ressourcen blev ikke fundet.")
        => NotFound(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Not Found",
            Detail = message
        });
}
