using FamilyHub.Api.Contracts.Catalog;
using FamilyHub.Api.Entities.Catalog;

namespace FamilyHub.Api.Features.Catalog;

internal static class ItemCategoryMappings
{
    internal static ItemCategoryListItemDto ToListItemDto(this ItemCategory category) => new(
        category.Id,
        category.Name,
        category.SortOrder);

    internal static ItemCategoryDetailsDto ToDetailsDto(this ItemCategory category) => new(
        category.Id,
        category.Name,
        category.SortOrder,
        category.CreatedAtUtc,
        category.UpdatedAtUtc);

    internal static ItemCategory ToEntity(this CreateItemCategoryRequest request) => new()
    {
        Name = request.Name,
        SortOrder = request.SortOrder
    };

    internal static void Apply(this ItemCategory category, UpdateItemCategoryRequest request)
    {
        category.Name = request.Name;
        category.SortOrder = request.SortOrder;
    }
}