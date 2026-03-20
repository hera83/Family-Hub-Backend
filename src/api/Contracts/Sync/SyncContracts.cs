using FamilyHub.Api.Contracts.Calendar;
using FamilyHub.Api.Contracts.Catalog;
using FamilyHub.Api.Contracts.Orders;
using FamilyHub.Api.Contracts.Recipes;

namespace FamilyHub.Api.Contracts.Sync;

public sealed record SyncManifestItemDto(
    string DatasetName,
    int TotalCount,
    DateTime? LastModifiedUtc
);

public sealed record SyncManifestDto(
    DateTime GeneratedAtUtc,
    IReadOnlyList<SyncManifestItemDto> Datasets
);

public sealed record FullSyncDto(
    DateTime GeneratedAtUtc,
    IReadOnlyList<FamilyMemberDetailsDto> FamilyMembers,
    IReadOnlyList<CalendarEventDetailsDto> CalendarEvents,
    IReadOnlyList<ItemCategoryDetailsDto> ItemCategories,
    IReadOnlyList<ProductDetailsDto> Products,
    IReadOnlyList<RecipeCategoryDetailsDto> RecipeCategories,
    IReadOnlyList<RecipeDetailsDto> Recipes,
    IReadOnlyList<RecipeIngredientDto> RecipeIngredients,
    IReadOnlyList<OrderDetailsDto> Orders,
    IReadOnlyList<OrderLineDto> OrderLines
);

public sealed record ChangesSinceDto(
    DateTime SinceUtc,
    DateTime GeneratedAtUtc,
    IReadOnlyList<FamilyMemberDetailsDto> FamilyMembers,
    IReadOnlyList<CalendarEventDetailsDto> CalendarEvents,
    IReadOnlyList<ItemCategoryDetailsDto> ItemCategories,
    IReadOnlyList<ProductDetailsDto> Products,
    IReadOnlyList<RecipeCategoryDetailsDto> RecipeCategories,
    IReadOnlyList<RecipeDetailsDto> Recipes,
    IReadOnlyList<RecipeIngredientDto> RecipeIngredients,
    IReadOnlyList<OrderDetailsDto> Orders,
    IReadOnlyList<OrderLineDto> OrderLines
);
