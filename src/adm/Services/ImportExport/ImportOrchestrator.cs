using ClosedXML.Excel;
using FamilyHub.Adm.Services.ImportExport.Handlers;
using FamilyHub.Adm.Services.ImportExport.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FamilyHub.Adm.Services.ImportExport;

public sealed class ImportOrchestrator : IImportOrchestrator
{
    // Preview and result data is held in memory for up to 15 minutes.
    // Adjust if users need more time between upload and confirm steps.
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(15);

    private readonly Dictionary<string, IImportHandler> _handlers;
    private readonly IMemoryCache _cache;

    public ImportOrchestrator(IEnumerable<IImportHandler> handlers, IMemoryCache cache)
    {
        _handlers = handlers.ToDictionary(h => h.ImportType, StringComparer.OrdinalIgnoreCase);
        _cache = cache;
    }

    public IReadOnlyList<(string TypeName, string DisplayName)> SupportedTypes =>
        _handlers.Values
            .OrderBy(h => h.DisplayName)
            .Select(h => (h.ImportType, h.DisplayName))
            .ToArray();

    public (string CacheKey, ImportPreview Preview) ParseAndCache(string importType, Stream fileStream)
    {
        if (!_handlers.TryGetValue(importType, out var handler))
            throw new ArgumentException($"Ukendt importtype: {importType}", nameof(importType));

        using var workbook = new XLWorkbook(fileStream);
        var preview = handler.Parse(workbook);

        var key = Guid.NewGuid().ToString("N");
        _cache.Set(key, preview, CacheExpiry);

        return (key, preview);
    }

    public ImportPreview? GetCachedPreview(string cacheKey)
        => _cache.TryGetValue<ImportPreview>(cacheKey, out var preview) ? preview : null;

    public async Task<(string ResultKey, ImportResult Result)> ExecuteAsync(
        string cacheKey, CancellationToken cancellationToken)
    {
        if (!_cache.TryGetValue<ImportPreview>(cacheKey, out var preview) || preview is null)
            throw new InvalidOperationException("Import-sessionen er udløbet. Upload filen igen.");

        if (!_handlers.TryGetValue(preview.TypeName, out var handler))
            throw new InvalidOperationException($"Handler ikke fundet for: {preview.TypeName}");

        var result = await handler.ExecuteAsync(preview, cancellationToken);

        var resultKey = Guid.NewGuid().ToString("N");
        _cache.Set(resultKey, result, CacheExpiry);

        return (resultKey, result);
    }

    public ImportResult? GetCachedResult(string resultKey)
        => _cache.TryGetValue<ImportResult>(resultKey, out var result) ? result : null;
}
