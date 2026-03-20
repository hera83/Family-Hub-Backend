namespace FamilyHub.Api.Contracts.Orders;

public sealed record OrderSyncDto(
    DateTime GeneratedAtUtc,
    IReadOnlyList<OrderDetailsDto> Orders,
    IReadOnlyList<OrderLineDto> OrderLines
);

public sealed record OrderChangesSinceDto(
    DateTime SinceUtc,
    DateTime GeneratedAtUtc,
    IReadOnlyList<OrderDetailsDto> Orders,
    IReadOnlyList<OrderLineDto> OrderLines
);
