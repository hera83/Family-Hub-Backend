using FamilyHub.Api.Contracts.Catalog;

namespace FamilyHub.Api.Features.Catalog;

public interface IProductRequestValidator
{
    void Validate(ProductListQueryRequest request);
    void Validate(CreateProductRequest request);
    void Validate(UpdateProductRequest request);
}
