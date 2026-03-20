namespace FamilyHub.Adm.Services.ImportExport;

public interface IImportExportService
{
    Task<ExcelExportFile> ExportFamilyMembersAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> ExportCalendarEventsAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> ExportItemCategoriesAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> ExportProductsAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> ExportRecipeCategoriesAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> ExportRecipesAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> ExportRecipeIngredientsAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> ExportOrdersAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> ExportOrderLinesAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> ExportWorkbookAsync(CancellationToken cancellationToken = default);
}
