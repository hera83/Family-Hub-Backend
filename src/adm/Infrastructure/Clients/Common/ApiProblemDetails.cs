namespace FamilyHub.Adm.Infrastructure.Clients.Common;

public sealed class ApiProblemDetails
{
    public int? Status { get; init; }
    public string? Title { get; init; }
    public string? Detail { get; init; }
}
