using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Services.ImportExport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.ImportExport;

public class IndexModel(
    IImportExportService importExportService,
    IExcelTemplateService excelTemplateService,
    ILogger<IndexModel> logger) : PageModel
{
    private readonly IImportExportService _importExportService = importExportService;
    private readonly IExcelTemplateService _excelTemplateService = excelTemplateService;
    private readonly ILogger<IndexModel> _logger = logger;

    public void OnGet()
    {
    }

    public Task<IActionResult> OnPostExportFamilyMembersAsync(CancellationToken cancellationToken)
        => ExecuteExportAsync(_importExportService.ExportFamilyMembersAsync, cancellationToken);

    public Task<IActionResult> OnPostExportCalendarEventsAsync(CancellationToken cancellationToken)
        => ExecuteExportAsync(_importExportService.ExportCalendarEventsAsync, cancellationToken);

    public Task<IActionResult> OnPostExportItemCategoriesAsync(CancellationToken cancellationToken)
        => ExecuteExportAsync(_importExportService.ExportItemCategoriesAsync, cancellationToken);

    public Task<IActionResult> OnPostExportProductsAsync(CancellationToken cancellationToken)
        => ExecuteExportAsync(_importExportService.ExportProductsAsync, cancellationToken);

    public Task<IActionResult> OnPostExportRecipeCategoriesAsync(CancellationToken cancellationToken)
        => ExecuteExportAsync(_importExportService.ExportRecipeCategoriesAsync, cancellationToken);

    public Task<IActionResult> OnPostExportRecipesAsync(CancellationToken cancellationToken)
        => ExecuteExportAsync(_importExportService.ExportRecipesAsync, cancellationToken);

    public Task<IActionResult> OnPostExportRecipeIngredientsAsync(CancellationToken cancellationToken)
        => ExecuteExportAsync(_importExportService.ExportRecipeIngredientsAsync, cancellationToken);

    public Task<IActionResult> OnPostExportOrdersAsync(CancellationToken cancellationToken)
        => ExecuteExportAsync(_importExportService.ExportOrdersAsync, cancellationToken);

    public Task<IActionResult> OnPostExportOrderLinesAsync(CancellationToken cancellationToken)
        => ExecuteExportAsync(_importExportService.ExportOrderLinesAsync, cancellationToken);

    public Task<IActionResult> OnPostExportWorkbookAsync(CancellationToken cancellationToken)
        => ExecuteExportAsync(_importExportService.ExportWorkbookAsync, cancellationToken);

    // ── Template downloads ───────────────────────────────────────────────────────

    public Task<IActionResult> OnPostDownloadTemplateFamilyMembersAsync(CancellationToken cancellationToken)
        => ExecuteTemplateAsync(_excelTemplateService.GetFamilyMembersTemplateAsync, cancellationToken);

    public Task<IActionResult> OnPostDownloadTemplateCalendarEventsAsync(CancellationToken cancellationToken)
        => ExecuteTemplateAsync(_excelTemplateService.GetCalendarEventsTemplateAsync, cancellationToken);

    public Task<IActionResult> OnPostDownloadTemplateItemCategoriesAsync(CancellationToken cancellationToken)
        => ExecuteTemplateAsync(_excelTemplateService.GetItemCategoriesTemplateAsync, cancellationToken);

    public Task<IActionResult> OnPostDownloadTemplateProductsAsync(CancellationToken cancellationToken)
        => ExecuteTemplateAsync(_excelTemplateService.GetProductsTemplateAsync, cancellationToken);

    public Task<IActionResult> OnPostDownloadTemplateRecipeCategoriesAsync(CancellationToken cancellationToken)
        => ExecuteTemplateAsync(_excelTemplateService.GetRecipeCategoriesTemplateAsync, cancellationToken);

    public Task<IActionResult> OnPostDownloadTemplateRecipesAsync(CancellationToken cancellationToken)
        => ExecuteTemplateAsync(_excelTemplateService.GetRecipesTemplateAsync, cancellationToken);

    public Task<IActionResult> OnPostDownloadTemplateRecipeIngredientsAsync(CancellationToken cancellationToken)
        => ExecuteTemplateAsync(_excelTemplateService.GetRecipeIngredientsTemplateAsync, cancellationToken);

    private async Task<IActionResult> ExecuteTemplateAsync(
        Func<CancellationToken, Task<ExcelExportFile>> generate,
        CancellationToken cancellationToken)
    {
        try
        {
            var file = await generate(cancellationToken);
            return File(file.Content, file.ContentType, file.FileName);
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate Excel template file.");
            TempData["ErrorMessage"] = "Kunne ikke generere template. Prøv igen om lidt.";
            return RedirectToPage();
        }
    }

    private async Task<IActionResult> ExecuteExportAsync(
        Func<CancellationToken, Task<ExcelExportFile>> export,
        CancellationToken cancellationToken)
    {
        try
        {
            var file = await export(cancellationToken);
            return File(file.Content, file.ContentType, file.FileName);
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate Excel export file.");
            TempData["ErrorMessage"] = "Eksport fejlede. Proev igen om lidt.";
            return RedirectToPage();
        }
    }
}
