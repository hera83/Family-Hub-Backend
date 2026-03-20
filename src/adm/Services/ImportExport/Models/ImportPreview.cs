namespace FamilyHub.Adm.Services.ImportExport.Models;

/// <summary>
/// A single parsed row from the uploaded Excel sheet.
/// </summary>
public sealed class ImportPreviewRow
{
    public required int RowNumber { get; init; }
    public bool IsValid { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];

    /// <summary>Column header → cell value pairs shown as table columns in the preview UI.</summary>
    public IReadOnlyList<KeyValuePair<string, string>> DisplayColumns { get; init; } = [];

    /// <summary>
    /// Typed domain object used by the importer when executing.
    /// Cast to the handler-specific type. Only populated on rows where IsValid == true.
    /// </summary>
    public object? Data { get; init; }
}

/// <summary>
/// Result of parsing an uploaded Excel file for a specific import data type.
/// Cached between the preview step and the execute step.
/// </summary>
public sealed class ImportPreview
{
    public required string TypeName { get; init; }
    public required string DisplayName { get; init; }
    public int TotalRows { get; init; }
    public int ValidRows { get; init; }
    public int InvalidRows { get; init; }
    public IReadOnlyList<ImportPreviewRow> Rows { get; init; } = [];
}
