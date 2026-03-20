using FamilyHub.Adm.Services.ImportExport;
using FamilyHub.Adm.Services.ImportExport.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.ImportExport;

public class ImportPreviewModel(IImportOrchestrator orchestrator, ILogger<ImportPreviewModel> logger) : PageModel
{
    private readonly IImportOrchestrator _orchestrator = orchestrator;
    private readonly ILogger<ImportPreviewModel> _logger = logger;

    private const int MaxDisplayRows = 150;

    public ImportPreview? Preview { get; private set; }
    public string? CacheKey { get; private set; }
    public IReadOnlyList<string> TableHeaders { get; private set; } = [];
    public IReadOnlyList<ImportPreviewRow> DisplayRows { get; private set; } = [];
    public bool HasMoreRowsThanDisplayed { get; private set; }
    public string? SessionExpiredMessage { get; private set; }

    public IActionResult OnGet()
    {
        CacheKey = TempData.Peek("ImportCacheKey") as string;
        if (string.IsNullOrWhiteSpace(CacheKey))
        {
            TempData["ErrorMessage"] = "Ingen import-session fundet. Upload en fil igen.";
            return RedirectToPage("/ImportExport/Import");
        }

        Preview = _orchestrator.GetCachedPreview(CacheKey);
        if (Preview is null)
        {
            SessionExpiredMessage = "Import-sessionen er udløbet (15 min). Upload filen igen.";
            return Page();
        }

        PrepareDisplayData();
        // Keep TempData alive for the POST handler
        TempData.Keep("ImportCacheKey");
        return Page();
    }

    public async Task<IActionResult> OnPostExecuteAsync(string cacheKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(cacheKey))
        {
            TempData["ErrorMessage"] = "Ingen import-session. Upload filen igen.";
            return RedirectToPage("/ImportExport/Import");
        }

        try
        {
            var (resultKey, _) = await _orchestrator.ExecuteAsync(cacheKey, cancellationToken);
            TempData["ImportResultKey"] = resultKey;
            return RedirectToPage("/ImportExport/ImportResult");
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToPage("/ImportExport/Import");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Import execution failed for cache key {CacheKey}.", cacheKey);
            TempData["ErrorMessage"] = "Import fejlede uventet. Prøv igen om lidt.";
            return RedirectToPage("/ImportExport/Import");
        }
    }

    private void PrepareDisplayData()
    {
        if (Preview is null) return;

        TableHeaders = Preview.Rows.FirstOrDefault()?.DisplayColumns.Select(kv => kv.Key).ToArray() ?? [];
        DisplayRows = Preview.Rows.Take(MaxDisplayRows).ToArray();
        HasMoreRowsThanDisplayed = Preview.Rows.Count > MaxDisplayRows;
    }
}
