using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Catalog;
using FamilyHub.Adm.Models.Api.Common;

namespace FamilyHub.Adm.Infrastructure.Clients.Catalog;

public sealed class CatalogApiClient(HttpClient httpClient) : ApiClientBase(httpClient), ICatalogApiClient
{
    public Task<IReadOnlyList<ItemCategoryListItemDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        => GetAsync<IReadOnlyList<ItemCategoryListItemDto>>("api/v1/catalog/categories", cancellationToken);

    public Task<ItemCategoryDetailsDto> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => GetAsync<ItemCategoryDetailsDto>($"api/v1/catalog/categories/{id}", cancellationToken);

    public Task<ItemCategoryDetailsDto> CreateCategoryAsync(CreateItemCategoryRequest request, CancellationToken cancellationToken = default)
        => PostAsync<CreateItemCategoryRequest, ItemCategoryDetailsDto>("api/v1/catalog/categories", request, cancellationToken);

    public Task<ItemCategoryDetailsDto> UpdateCategoryAsync(Guid id, UpdateItemCategoryRequest request, CancellationToken cancellationToken = default)
        => PutAsync<UpdateItemCategoryRequest, ItemCategoryDetailsDto>($"api/v1/catalog/categories/{id}", request, cancellationToken);

    public Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
        => DeleteAsync($"api/v1/catalog/categories/{id}", cancellationToken);

    public Task<PagedListResponse<ProductListItemDto>> GetProductsAsync(ProductListQueryRequest? query = null, CancellationToken cancellationToken = default)
    {
        var effectiveQuery = query ?? new ProductListQueryRequest();

        var uri = WithQueryString("api/v1/catalog/products", new Dictionary<string, string?>
        {
            ["search"] = effectiveQuery.Search,
            ["itemCategoryId"] = effectiveQuery.ItemCategoryId?.ToString(),
            ["favoritesOnly"] = effectiveQuery.FavoritesOnly ? "true" : null,
            ["staplesOnly"] = effectiveQuery.StaplesOnly ? "true" : null,
            ["page"] = effectiveQuery.Page.ToString(),
            ["pageSize"] = effectiveQuery.PageSize.ToString()
        });

        return GetAsync<PagedListResponse<ProductListItemDto>>(uri, cancellationToken);
    }

    public Task<ProductDetailsDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => GetAsync<ProductDetailsDto>($"api/v1/catalog/products/{id}", cancellationToken);

    public Task<ProductDetailsDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
        => PostAsync<CreateProductRequest, ProductDetailsDto>("api/v1/catalog/products", request, cancellationToken);

    public Task<ProductDetailsDto> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
        => PutAsync<UpdateProductRequest, ProductDetailsDto>($"api/v1/catalog/products/{id}", request, cancellationToken);

    public Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
        => DeleteAsync($"api/v1/catalog/products/{id}", cancellationToken);
}
