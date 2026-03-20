using FamilyHub.Adm.Models.Api.Calendar;
using FamilyHub.Adm.Models.Api.Catalog;
using FamilyHub.Adm.Models.Api.Orders;
using FamilyHub.Adm.Models.Api.Recipes;

namespace FamilyHub.Adm.Models.Api.Sync;

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
