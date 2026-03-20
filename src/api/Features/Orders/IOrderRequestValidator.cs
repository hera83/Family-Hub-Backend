using FamilyHub.Api.Contracts.Orders;

namespace FamilyHub.Api.Features.Orders;

public interface IOrderRequestValidator
{
    void Validate(OrderListQueryRequest request);
}
