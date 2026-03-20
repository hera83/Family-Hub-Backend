using FamilyHub.Adm.Services.ImportExport;
using FamilyHub.Adm.Services.ImportExport.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHub.Adm.Pages.ImportExport;

public class ImportModel(IImportOrchestrator orchestrator, ILogger<ImportModel> logger) : PageModel
{
    private readonly IImportOrchestrator _orchestrator = orchestrator;
    private readonly ILogger<ImportModel> _logger = logger;

    public IReadOnlyList<SelectListItem> ImportTypeOptions { get; private set; } = [];

    [BindProperty]
    public string? SelectedImportType { get; set; }

    public void OnGet()
    {
        LoadTypeOptions();
    }

    public async Task<IActionResult> OnPostAsync(IFormFile? importFile, CancellationToken cancellationToken)
    {
        LoadTypeOptions();

        if (string.IsNullOrWhiteSpace(SelectedImportType))
        {
            ModelState.AddModelError(nameof(SelectedImportType), "Vælg en datatype.");
            return Page();
        }

        if (importFile is null || importFile.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Vælg en Excel-fil (.xlsx).");
            return Page();
        }

        if (!importFile.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(string.Empty, "Kun .xlsx filer understøttes.");
            return Page();
        }

        try
        {
            using var stream = importFile.OpenReadStream();
            var (cacheKey, _) = _orchestrator.ParseAndCache(SelectedImportType, stream);

            TempData["ImportCacheKey"] = cacheKey;
            return RedirectToPage("/ImportExport/ImportPreview");
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse uploaded import file.");
            ModelState.AddModelError(string.Empty, "Filen kunne ikke læses. Kontroller at det er en gyldig .xlsx fil.");
            return Page();
        }
    }

    private void LoadTypeOptions()
    {
        ImportTypeOptions = _orchestrator.SupportedTypes
            .Select(t => new SelectListItem(t.DisplayName, t.TypeName))
            .ToArray();
    }
}
