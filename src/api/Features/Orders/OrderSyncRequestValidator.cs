using FamilyHub.Api.Features.Common;

namespace FamilyHub.Api.Features.Orders;

internal sealed class OrderSyncRequestValidator : IOrderSyncRequestValidator
{
    public DateTime ValidateAndParseSinceUtc(string? sinceUtc)
        => SinceUtcParser.ValidateAndParse(sinceUtc);
}
