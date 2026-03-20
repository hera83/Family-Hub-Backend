using FamilyHub.Adm.Models.Api.Sync;

namespace FamilyHub.Adm.Models.Dashboard;

public sealed class DashboardViewModel
{
    public DateTime CheckedAtUtc { get; init; }
    public int? FamilyMembersCount { get; init; }
    public int? CalendarEventsCount { get; init; }
    public int? ItemCategoriesCount { get; init; }
    public int? ProductsCount { get; init; }
    public int? RecipeCategoriesCount { get; init; }
    public int? RecipesCount { get; init; }
    public int? RecipeIngredientsCount { get; init; }
    public int? OrdersCount { get; init; }
    public int? OrderLinesCount { get; init; }
    public SyncManifestDto? Manifest { get; init; }
    public string? ErrorMessage { get; init; }

    public bool IsConnected => string.IsNullOrWhiteSpace(ErrorMessage);

    /// <summary>Returns the last-modified timestamp for a manifest dataset by its API name.</summary>
    public DateTime? GetLastModified(string datasetName)
        => Manifest?.Datasets.FirstOrDefault(d => d.DatasetName == datasetName)?.LastModifiedUtc;
}
