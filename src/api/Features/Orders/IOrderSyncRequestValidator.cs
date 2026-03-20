namespace FamilyHub.Api.Features.Orders;

public interface IOrderSyncRequestValidator
{
    DateTime ValidateAndParseSinceUtc(string? sinceUtc);
}
