namespace FamilyHub.Api.Entities.Orders;

/// <summary>
/// Gyldige statusværdier for en ordre i v1.
/// </summary>
public static class OrderStatus
{
    public const string Created   = "Created";
    public const string Confirmed = "Confirmed";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";

    public static readonly IReadOnlyList<string> All =
        [Created, Confirmed, Completed, Cancelled];

    public static bool IsValid(string? value) =>
        value is not null && All.Contains(value);
}
