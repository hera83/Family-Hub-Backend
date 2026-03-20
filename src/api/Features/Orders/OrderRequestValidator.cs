using FamilyHub.Api.Contracts.Orders;
using FamilyHub.Api.Entities.Orders;
using FamilyHub.Api.Features.Common;

namespace FamilyHub.Api.Features.Orders;

internal sealed class OrderRequestValidator : IOrderRequestValidator
{
    public void Validate(OrderListQueryRequest request)
    {
        PaginationValidator.Validate(request.Page, request.PageSize);

        if (request.Status is not null && !OrderStatus.IsValid(request.Status))
            throw new ArgumentException($"Ugyldig status. Gyldige værdier: {string.Join(", ", OrderStatus.All)}.");
    }
}
