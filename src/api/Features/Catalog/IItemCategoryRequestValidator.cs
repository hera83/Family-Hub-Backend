using FamilyHub.Api.Contracts.Catalog;

namespace FamilyHub.Api.Features.Catalog;

public interface IItemCategoryRequestValidator
{
    void Validate(CreateItemCategoryRequest request);
    void Validate(UpdateItemCategoryRequest request);
}
