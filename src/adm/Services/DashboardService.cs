using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Sync;
using FamilyHub.Adm.Models.Dashboard;

namespace FamilyHub.Adm.Services;

/// <summary>
/// Loads dashboard data from a single sync/manifest API call.
/// The manifest returns TotalCount + LastModifiedUtc per dataset, which is
/// everything the dashboard needs without hammering 7 separate endpoints.
/// </summary>
public sealed class DashboardService(
    ISyncApiClient syncApiClient,
    ILogger<DashboardService> logger) : IDashboardService
{
    private readonly ISyncApiClient _syncApiClient = syncApiClient;
    private readonly ILogger<DashboardService> _logger = logger;

    // Dataset names as used by the API manifest endpoint.
    private const string DS_FamilyMembers      = "familyMembers";
    private const string DS_CalendarEvents     = "calendarEvents";
    private const string DS_ItemCategories     = "itemCategories";
    private const string DS_Products           = "products";
    private const string DS_RecipeCategories   = "recipeCategories";
    private const string DS_Recipes            = "recipes";
    private const string DS_RecipeIngredients  = "recipeIngredients";
    private const string DS_Orders             = "orders";
    private const string DS_OrderLines         = "orderLines";

    public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var manifest = await GetManifestWithStartupRetryAsync(cancellationToken);
            var ds = manifest.Datasets.ToDictionary(d => d.DatasetName, d => d);

            return new DashboardViewModel
            {
                CheckedAtUtc          = DateTime.UtcNow,
                FamilyMembersCount    = ds.TryGetValue(DS_FamilyMembers,     out var fm)  ? fm.TotalCount  : null,
                CalendarEventsCount   = ds.TryGetValue(DS_CalendarEvents,    out var ce)  ? ce.TotalCount  : null,
                ItemCategoriesCount   = ds.TryGetValue(DS_ItemCategories,    out var ic)  ? ic.TotalCount  : null,
                ProductsCount         = ds.TryGetValue(DS_Products,          out var p)   ? p.TotalCount   : null,
                RecipeCategoriesCount = ds.TryGetValue(DS_RecipeCategories,  out var rca) ? rca.TotalCount : null,
                RecipesCount          = ds.TryGetValue(DS_Recipes,           out var r)   ? r.TotalCount   : null,
                RecipeIngredientsCount= ds.TryGetValue(DS_RecipeIngredients, out var ri)  ? ri.TotalCount  : null,
                OrdersCount           = ds.TryGetValue(DS_Orders,            out var o)   ? o.TotalCount   : null,
                OrderLinesCount       = ds.TryGetValue(DS_OrderLines,        out var ol)  ? ol.TotalCount  : null,
                Manifest              = manifest,
            };
        }
        catch (ApiClientException ex)
        {
            _logger.LogWarning(ex, "API error while loading dashboard from api");
            return new DashboardViewModel { CheckedAtUtc = DateTime.UtcNow, ErrorMessage = ex.UserMessage };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while loading dashboard from api");

            var detail = ex.Message;
            if (ex.InnerException is not null)
            {
                detail = $"{detail} ({ex.InnerException.Message})";
            }

            return new DashboardViewModel
            {
                CheckedAtUtc = DateTime.UtcNow,
                ErrorMessage = $"Kunne ikke kontakte api på den konfigurerede BaseUrl. {detail}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load dashboard data from api");
            return new DashboardViewModel
            {
                CheckedAtUtc = DateTime.UtcNow,
                ErrorMessage = "Kunne ikke hente data fra api. Tjek BaseUrl, API key og at api-processen kører."
            };
        }
    }

    private async Task<Models.Api.Sync.SyncManifestDto> GetManifestWithStartupRetryAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _syncApiClient.GetManifestAsync(cancellationToken);
        }
        catch (HttpRequestException)
        {
            // Compound debug start can bring ADM up a bit before API is listening.
            await Task.Delay(TimeSpan.FromMilliseconds(900), cancellationToken);
            return await _syncApiClient.GetManifestAsync(cancellationToken);
        }
    }
}
