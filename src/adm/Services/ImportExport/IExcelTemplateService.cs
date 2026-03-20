namespace FamilyHub.Adm.Services.ImportExport;

/// <summary>
/// Generates downloadable Excel template files for each importable data type.
/// Each template contains a data sheet with the correct column headers,
/// an example row, a "Hjælp" sheet with field-level documentation and import rules,
/// and optionally a lookup sheet with live FK reference data from the API.
/// </summary>
public interface IExcelTemplateService
{
    Task<ExcelExportFile> GetFamilyMembersTemplateAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> GetCalendarEventsTemplateAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> GetItemCategoriesTemplateAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> GetProductsTemplateAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> GetRecipeCategoriesTemplateAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> GetRecipesTemplateAsync(CancellationToken cancellationToken = default);
    Task<ExcelExportFile> GetRecipeIngredientsTemplateAsync(CancellationToken cancellationToken = default);
}
