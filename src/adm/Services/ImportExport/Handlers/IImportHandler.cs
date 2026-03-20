using ClosedXML.Excel;
using FamilyHub.Adm.Services.ImportExport.Models;

namespace FamilyHub.Adm.Services.ImportExport.Handlers;

/// <summary>
/// Handles parsing and execution for a specific import data type.
/// Register implementations with DI as IImportHandler to make them available in ImportOrchestrator.
/// </summary>
public interface IImportHandler
{
    string ImportType { get; }
    string DisplayName { get; }

    /// <summary>
    /// Reads the workbook and returns a preview with parsed rows and validation state.
    /// Must not modify any data or call the API.
    /// </summary>
    ImportPreview Parse(IXLWorkbook workbook);

    /// <summary>
    /// Executes the import for all valid rows in the preview by calling the API.
    /// Invalid rows are skipped. Errors on individual rows are collected rather than aborting.
    /// </summary>
    Task<ImportResult> ExecuteAsync(ImportPreview preview, CancellationToken cancellationToken);
}
