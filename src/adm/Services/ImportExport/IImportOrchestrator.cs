using FamilyHub.Adm.Services.ImportExport.Models;

namespace FamilyHub.Adm.Services.ImportExport;

/// <summary>
/// Coordinates the import flow: parsing, caching, and executing imports per data type.
/// Extend by registering new IImportHandler implementations in DI.
/// </summary>
public interface IImportOrchestrator
{
    /// <summary>All supported import types with their display names, in display-name order.</summary>
    IReadOnlyList<(string TypeName, string DisplayName)> SupportedTypes { get; }

    /// <summary>
    /// Parses the uploaded Excel file for the given type, caches the preview, and returns the result.
    /// Returns the cache key used to retrieve the preview in subsequent steps.
    /// Throws ArgumentException if the type is not supported.
    /// </summary>
    (string CacheKey, ImportPreview Preview) ParseAndCache(string importType, Stream fileStream);

    /// <summary>Retrieves a cached preview by its key. Returns null if the session has expired (15 min TTL).</summary>
    ImportPreview? GetCachedPreview(string cacheKey);

    /// <summary>
    /// Executes the import for the preview identified by cacheKey.
    /// Returns the result and a result cache key for the result page.
    /// Throws InvalidOperationException if the preview session has expired.
    /// </summary>
    Task<(string ResultKey, ImportResult Result)> ExecuteAsync(string cacheKey, CancellationToken cancellationToken);

    /// <summary>Retrieves a cached import result by its key. Returns null if expired.</summary>
    ImportResult? GetCachedResult(string resultKey);
}
